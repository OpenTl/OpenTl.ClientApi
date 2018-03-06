namespace OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs
{
    using System.Collections.Generic;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Common.Utilities;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(IHandshakeHandler))]
    internal class HanshakeResponseDecoder: ByteToMessageDecoder, IHandshakeHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HanshakeResponseDecoder));

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.GetLongLE(0) != 0)
            {
                context.FireChannelRead(input.Retain());
                return;
            }

            input.SetReaderIndex(8);
            
            var messageId = input.ReadLongLE();
            var dataLength = input.ReadIntLE();

            var message = Serializer.Deserialize(input);
        
            Log.Debug($"Recieve the message {message} with id: {messageId}");
        
            output.Add(message);
        }
    }
}