namespace OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Common.Utilities;
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

        public override bool AcceptOutboundMessage(object message) => message is RequestReqPqMulti || message is RequestReqDHParams || message is RequestSetClientDHParams;

        protected override void Encode(IChannelHandlerContext context, IObject message, IByteBuffer output)
        {
            var newMessageId = ClientSettings.ClientSession.GenerateMessageId();
            SessionWriter.Save(ClientSettings.ClientSession);
            
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Send handshake message {message} with id : {newMessageId}");

            output.WriteLongLE(0);
            output.WriteLongLE(newMessageId);

            var dataBuffer = Serializer.Serialize(message);
            try
            {
                output.WriteIntLE(dataBuffer.ReadableBytes);
                output.WriteBytes(dataBuffer);
            }
            finally
            {
                dataBuffer.SafeRelease();
            }
        }
    }
}