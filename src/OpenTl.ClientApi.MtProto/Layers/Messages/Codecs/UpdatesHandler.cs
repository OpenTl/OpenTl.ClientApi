namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class UpdatesHandler : SimpleChannelInboundHandler<IUpdates>,
                                    IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpdatesHandler));

        public int Order { get; } = 100;

        // public IUpdatesApiServiceRaiser UpdateRaiser { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IUpdates msg)
        {
            // if (Log.IsDebugEnabled)
            // {
            //     var jUpdate = JsonConvert.SerializeObject(obj);
            //     Log.Debug($"Recieve Updates \n{jUpdate}");
            // }

            // // UpdateRaiser.OnUpdateRecieve(obj.Cast<IUpdates>());
        }
    }
}