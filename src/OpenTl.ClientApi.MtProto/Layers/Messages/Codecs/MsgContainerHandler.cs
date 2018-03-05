namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using System.Collections.Generic;
    using System.Linq;

    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class MsgContainerHandler: MessageToMessageDecoder<TMsgContainer>, IMessageHandler
    {
        protected override void Decode(IChannelHandlerContext context, TMsgContainer message, List<object> output)
        {
             output.AddRange(message.Messages.Select(m => m.Body)); 
        }
    }
}