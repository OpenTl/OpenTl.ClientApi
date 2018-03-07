namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class PongHandler : SimpleChannelInboundHandler<TPong>,
                                 IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PongHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, TPong msg)
        {
            Log.Debug("Handle ping");
        }
    }
}
