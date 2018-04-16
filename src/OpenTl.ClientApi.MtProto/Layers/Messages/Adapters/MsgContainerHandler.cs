namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System.Collections.Generic;
    using System.Linq;

    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class MsgContainerHandler : MessageToMessageDecoder<MsgContainer>,
                                         IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MsgContainerHandler));

        public int Order { get; } = 0;

        public override bool IsSharable { get; } = true;
        
        public IClientSettings ClientSettings { get; set; }

        protected override void Decode(IChannelHandlerContext context, MsgContainer message, List<object> output)
        {
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Process MsgContainer message with {message.Messages.Length} items");

            output.AddRange(message.Messages.Select(m => m.Body));
        }
    }
}
