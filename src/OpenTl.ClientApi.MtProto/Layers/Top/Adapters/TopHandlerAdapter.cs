namespace OpenTl.ClientApi.MtProto.Layers.Top.Adapters
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using DotNetty.Transport.Channels;

    using log4net;
    using log4net.Util;

    using OpenTl.ClientApi.MtProto.Enums;
    using OpenTl.ClientApi.MtProto.Extensions;
    using OpenTl.ClientApi.MtProto.Interfaces;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Schema.Help;

    [SingleInstance(typeof(ITopLevelHandler), typeof(IContextGetter))]
    internal sealed class TopHandlerAdapter : SimpleChannelInboundHandler<IObject>,
                                              ITopLevelHandler,
                                              IContextGetter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TopHandlerAdapter));

        public IChannelHandlerContext Context { get; private set; }

        public IRequestService RequestService { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public override bool IsSharable { get; } = true;


        public override void ChannelActive(IChannelHandlerContext context)
        {
            Context = context;

            base.ChannelActive(context);

            if (ClientSettings.ClientSession.SessionWasHandshaked())
            {
                Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Session was found.");

                UserEventTriggered(context, ESystemNotification.HandshakeComplete);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.ErrorExt($"#{ClientSettings.ClientSession.SessionId}: Caught exception", exception);

            RequestService.ReturnException(exception);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is ESystemNotification.HandshakeComplete)
            {
                Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Handshake is complete");

                SendInitConnectionRequest().ConfigureAwait(false);
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IObject msg)
        {
            Log.Error($"#{ClientSettings.ClientSession.SessionId}: Unhandled message {msg}");
        }

        private async Task SendInitConnectionRequest()
        {
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Send init connection request");

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

                await Context.WriteAndFlushAsync(request).ConfigureAwait(false);

                ClientSettings.Config = (IConfig)await resultTask.ConfigureAwait(false);

                if (!ClientSettings.UseIPv6)
                {
                    var filtredDc = ClientSettings.Config.DcOptions.Where(dc => !dc.Ipv6).ToArray();
                    ClientSettings.Config.DcOptions = new TVector<IDcOption>(filtredDc);
                }
                
                foreach (var replyRequest in RequestService.GetAllRequestToReply())
                {
#pragma warning disable 4014
                    Context.WriteAsync(replyRequest);
#pragma warning restore 4014
                }

                Context.Flush();
            }
            catch (Exception e)
            {
                RequestService.ReturnException(e);
            }
        }
    }
}