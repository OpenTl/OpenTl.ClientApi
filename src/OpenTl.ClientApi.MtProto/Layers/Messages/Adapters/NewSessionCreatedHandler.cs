namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
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

        public override bool IsSharable { get; } = true;
        
        public IClientSettings ClientSettings { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, TNewSessionCreated msg)
        {
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Handle a new session was created");
        }
    }
}
