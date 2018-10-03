using log4net;

namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using DotNetty.Transport.Channels;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class UpdatesHandler : SimpleChannelInboundHandler<IUpdates>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpdatesHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;

        public IAutoUpdatesHandler AutoUpdatesHandler { get; set; }
        
        public IClientSettings ClientSettings { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IUpdates msg)
        {
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Process updates");

            AutoUpdatesHandler.HandleAutoUpdates(msg);
        }
    }
}