namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class MsgsAckHandler: SimpleChannelInboundHandler<TMsgsAck>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MsgsAckHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TMsgsAck msg)
        {
            Log.Debug("Handle a messages ack");

            if (Log.IsDebugEnabled)
            {
                var jMessages = JsonConvert.SerializeObject(msg.MsgIds.Items);
                Log.Debug($"Receiving confirmation of the messages: {jMessages}");
            }
        }
    }
}