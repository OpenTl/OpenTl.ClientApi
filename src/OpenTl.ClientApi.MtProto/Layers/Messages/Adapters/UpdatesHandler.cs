namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class UpdatesHandler : SimpleChannelInboundHandler<IUpdates>,
                                    IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpdatesHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        public IUpdatesRaiser UpdateRaiser { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IUpdates msg)
        {
            if (Log.IsDebugEnabled)
            {
                var jUpdate = JsonConvert.SerializeObject(msg);
                Log.Debug($"Recieve Updates \n{jUpdate}");
            }

            UpdateRaiser.OnUpdateRecieve(msg);
        }
    }
}