namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class MsgDetailedInfoHandler: SimpleChannelInboundHandler<TMsgDetailedInfo>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MsgDetailedInfoHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMsgDetailedInfo msg)
        {
            Log.Debug("Handle MsgDetailedInfo");
        }
    }
}