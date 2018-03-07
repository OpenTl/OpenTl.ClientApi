namespace OpenTl.ClientApi.MtProto.Layers.Secure.Codecs
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.ClientApi.MtProto.Extensions;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Common.MtProto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(ISecureHandler))]
    internal sealed class SecureRequestEncoder: MessageToByteEncoder<IRequest>,
                                         ISecureHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecureRequestEncoder));

        public IClientSettings ClientSettings { get; set; }

        public IRequestService RequestService { get; set; }
        
        public override bool IsSharable { get; } = true;

        public override bool AcceptOutboundMessage(object message) => message is IRequest;

        protected override void Encode(IChannelHandlerContext context, IRequest message, IByteBuffer output)
        {
            var messageId = ClientSettings.ClientSession.GenerateMessageId();
            var sequenceNumber = ClientSettings.ClientSession.GenerateSequenceNumber(true);
            
            Log.Debug($"Send secure message with messageId = {messageId} and sequenceNumber = {sequenceNumber}");
            
            var messageBuffer = PooledByteBufferAllocator.Default.Buffer();
            try
            {
                Serializer.Serialize(message, messageBuffer);
            
                MtProtoHelper.ToServerEncrypt(messageBuffer, ClientSettings.ClientSession, messageId, sequenceNumber, output);
            }
            finally
            {
                messageBuffer.Release();
            }
            
            RequestService.AttachRequestToMessageId(message, messageId);
        }
    }
}