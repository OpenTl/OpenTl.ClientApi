namespace OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.ClientApi.MtProto.Extensions;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(IHandshakeHandler))]
    internal sealed class HandshakeRequestEncoder: MessageToByteEncoder<IObject>,
                                                 IHandshakeHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HandshakeRequestEncoder));

        public IClientSettings ClientSettings { get; set; }

        public ISessionWriter SessionWriter { get; set; } 
            
        public override bool IsSharable { get; } = true;

        public override bool AcceptOutboundMessage(object message) => message is RequestReqPqMulty || message is RequestReqDHParams || message is RequestSetClientDHParams;

        protected override void Encode(IChannelHandlerContext context, IObject message, IByteBuffer output)
        {
            var newMessageId = ClientSettings.ClientSession.GenerateMessageId();
            SessionWriter.Save(ClientSettings.ClientSession);
            
            Log.Debug($"Send handshake message {message} with id : {newMessageId}");

            var dataBuffer = PooledByteBufferAllocator.Default.Buffer();
            Serializer.Serialize(message, dataBuffer);

            output.WriteLongLE(0);
            output.WriteLongLE(newMessageId);
            output.WriteIntLE(dataBuffer.ReadableBytes);
            output.WriteBytes(dataBuffer);
        }
    }
}