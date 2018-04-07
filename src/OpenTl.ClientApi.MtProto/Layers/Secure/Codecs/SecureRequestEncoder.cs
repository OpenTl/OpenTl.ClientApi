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
    internal sealed class SecureRequestEncoder: MessageToByteEncoder<IObject>,
                                         ISecureHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecureRequestEncoder));

        public IClientSettings ClientSettings { get; set; }

        public IRequestService RequestService { get; set; }
        
        public ISessionWriter SessionWriter { get; set; } 

        public override bool IsSharable { get; } = true;

        public override bool AcceptOutboundMessage(object message) => message is IObject;

        protected override void Encode(IChannelHandlerContext context, IObject message, IByteBuffer output)
        {
            if (message == null)
            {
                return;
            }
            
            var messageId = ClientSettings.ClientSession.GenerateMessageId();
            
            var isRequest = message is IRequest;
            
            var sequenceNumber = ClientSettings.ClientSession.GenerateSequenceNumber(isRequest);
            
            SessionWriter.Save(ClientSettings.ClientSession);
            
            var messageBuffer = Serializer.Serialize(message);

            try
            {
                MtProtoHelper.ToServerEncrypt(messageBuffer, ClientSettings.ClientSession, messageId, sequenceNumber, output);
            }
            finally
            {
                messageBuffer.Release();
            }

            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Send secure message {message} with messageId = {messageId} and sequenceNumber = {sequenceNumber}");
            
            if (isRequest)
            {
                RequestService.AttachRequestToMessageId(message.Is<IRequest>(), messageId);
            }
        }
    }
}