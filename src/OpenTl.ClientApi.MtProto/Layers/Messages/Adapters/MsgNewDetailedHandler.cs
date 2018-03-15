namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class MsgNewDetailedHandler : SimpleChannelInboundHandler<TMsgNewDetailedInfo>,
                                           IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MsgNewDetailedHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        public IClientSettings ClientSettings { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMsgNewDetailedInfo msg)
        {
            if (Log.IsDebugEnabled)
            {
                var jMessages = JsonConvert.SerializeObject(msg);
                Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Receiving MsgNewDetailedInfo message: {jMessages}");
            }
        }
    }
}
