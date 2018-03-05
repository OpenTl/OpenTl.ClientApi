namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class PingHandler: SimpleChannelInboundHandler<RequestPing>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PingHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, RequestPing msg)
        {
            Log.Debug("Handle ping");
        }
    }
}