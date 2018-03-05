namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class PongHandler: SimpleChannelInboundHandler<TPong>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PongHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TPong msg)
        {
            Log.Debug("Handle ping");
        }
    }
}