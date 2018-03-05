namespace OpenTl.ClientApi.MtProto.Layers.Top.Adapters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels;

    using log4net;
    using log4net.Util;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(ITopLevelHandler), typeof(IPackageSender))]
    internal class MtProtoHandlerAdapter: SimpleChannelInboundHandler<IObject>,
                                          ITopLevelHandler,
                                          IPackageSender
    {

        public IRequestService RequestService { get; set; }
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(MtProtoHandlerAdapter));

        private IChannelHandlerContext _context;

        private TaskCompletionSource<bool> _initTask;
        // public IClientSettings ClientSettings { get; set; }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _context = context;
            
            _initTask?.SetCanceled();
            
            _initTask = new TaskCompletionSource<bool>();
            
            base.ChannelActive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IObject msg)
        {
            switch (msg)
            {
                case TDhGenOk _:
                    _initTask.SetResult(true);                    
                    break;
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.ErrorExt($"Caught exception", exception);

            base.ExceptionCaught(context, exception);
        }

        public async Task<TResult> SendRequest<TResult>(IRequest<TResult> request)
        {
            if (!_initTask.Task.IsCanceled && !_initTask.Task.IsCompleted)
            {
                await _initTask.Task;
            }
            
            await _context.WriteAndFlushAsync(request);
            
            return (TResult) await RequestService.RegisterRequest(request, CancellationToken.None);
        }
    }
}