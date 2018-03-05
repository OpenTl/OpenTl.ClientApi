namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class NewSessionCreatedHandler: SimpleChannelInboundHandler<TMsgsAck>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NewSessionCreatedHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMsgsAck msg)
        {
            Log.Debug("Handle a new session was created");
        }
    }

}