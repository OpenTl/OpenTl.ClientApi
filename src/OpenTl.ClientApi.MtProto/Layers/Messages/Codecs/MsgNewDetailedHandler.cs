namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class MsgNewDetailedHandler: SimpleChannelInboundHandler<TMsgNewDetailedInfo>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MsgNewDetailedHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMsgNewDetailedInfo msg)
        {
            Log.Debug("Handle a MsgNewDetailedInfo");
        }
    }
}