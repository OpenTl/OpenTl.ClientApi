namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class NewSessionCreatedHandler : SimpleChannelInboundHandler<TNewSessionCreated>,
                                              IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NewSessionCreatedHandler));

        public int Order { get; } = 100;

        protected override void ChannelRead0(IChannelHandlerContext ctx, TNewSessionCreated msg)
        {
            Log.Debug("Handle a new session was created");
        }
    }
}
