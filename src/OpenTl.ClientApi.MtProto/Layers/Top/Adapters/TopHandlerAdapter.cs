namespace OpenTl.ClientApi.MtProto.Layers.Top.Adapters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using DotNetty.Transport.Channels;

    using log4net;
    using log4net.Util;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Help;

    [SingleInstance(typeof(ITopLevelHandler), typeof(IPackageSender))]
    internal sealed class TopHandlerAdapter : SimpleChannelInboundHandler<IObject>,
                                       ITopLevelHandler,
                                       IPackageSender
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TopHandlerAdapter));

        private IChannelHandlerContext _context;

        private TaskCompletionSource<bool> _initTask = new TaskCompletionSource<bool>();

        public IRequestService RequestService { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _context = context;

            if (_initTask.Task.IsCanceled || _initTask.Task.IsCompleted || _initTask.Task.IsFaulted)
            {
                _initTask = new TaskCompletionSource<bool>();
            }

            base.ChannelActive(context);

            if (ClientSettings.ClientSession.AuthKey != null)
            {
                Log.Debug("Session was found.");
                
                SendInitConnectionRequest().ConfigureAwait(false);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.ErrorExt($"Caught exception", exception);

            RequestService.ReturnException(exception);
        }

        public async Task<TResult> SendRequest<TResult>(IRequest<TResult> request)
        {
            if (!_initTask.Task.IsCanceled && !_initTask.Task.IsCompleted)
            {
                await _initTask.Task;
            }

            var resultTask = RequestService.RegisterRequest(request, CancellationToken.None);

            await _context.WriteAndFlushAsync(request);

            return (TResult)await resultTask;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IObject msg)
        {
            switch (msg)
            {
                case TDhGenOk _:
                    SendInitConnectionRequest().ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task SendInitConnectionRequest()
        {
            Log.Debug("Send init connection request");
            
            try
            {
                Guard.That(ClientSettings.AppId).IsNotDefault();
                Guard.That(ClientSettings.ApplicationProperties.AppVersion).IsNotNullOrWhiteSpace();
                Guard.That(ClientSettings.ApplicationProperties.DeviceModel).IsNotNullOrWhiteSpace();
                Guard.That(ClientSettings.ApplicationProperties.LangCode).IsNotNullOrWhiteSpace();
                Guard.That(ClientSettings.ApplicationProperties.LangPack).IsNotNullOrWhiteSpace();
                Guard.That(ClientSettings.ApplicationProperties.SystemLangCode).IsNotNullOrWhiteSpace();
                Guard.That(ClientSettings.ApplicationProperties.SystemVersion).IsNotNullOrWhiteSpace();

                var request = new RequestInvokeWithLayer
                              {
                                  Layer = SchemaInfo.SchemaVersion,
                                  Query = new RequestInitConnection
                                          {
                                              ApiId = ClientSettings.AppId,
                                              AppVersion = ClientSettings.ApplicationProperties.AppVersion,
                                              DeviceModel = ClientSettings.ApplicationProperties.DeviceModel,
                                              LangCode = ClientSettings.ApplicationProperties.LangCode,
                                              LangPack = ClientSettings.ApplicationProperties.LangPack,
                                              SystemLangCode = ClientSettings.ApplicationProperties.SystemLangCode,
                                              Query = new RequestGetConfig(),
                                              SystemVersion = ClientSettings.ApplicationProperties.SystemVersion
                                          }
                              };

                var resultTask = RequestService.RegisterRequest(request, CancellationToken.None);

                await _context.WriteAndFlushAsync(request).ConfigureAwait(false);

                ClientSettings.Config = (IConfig)await resultTask;

                _initTask.SetResult(true);
            }
            catch (Exception e)
            {
                _initTask.SetException(e);
            }
        }
    }
}