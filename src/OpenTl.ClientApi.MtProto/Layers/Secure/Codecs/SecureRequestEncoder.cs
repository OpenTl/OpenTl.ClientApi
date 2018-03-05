namespace OpenTl.ClientApi.MtProto.Layers.Secure.Codecs
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Common.MtProto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(ISecureHandler))]
    internal class SecureRequestEncoder: MessageToByteEncoder<IRequest>,
                                         ISecureHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecureRequestEncoder));

        public IClientSettings ClientSettings { get; set; }

        public override bool AcceptOutboundMessage(object message) => message is IRequest;

        protected override void Encode(IChannelHandlerContext context, IRequest message, IByteBuffer output)
        {
            (long mesId, int seqNo) = ClientSettings.ClientSession.GenerateMsgIdAndSeqNo(true);

            Log.Debug($"Send secure message with Id = {mesId} and seqNo = {seqNo}");
            
            var messageBuffer = PooledByteBufferAllocator.Default.Buffer();
            Serializer.Serialize(message, messageBuffer);
            
            MtProtoHelper.ToServerEncrypt(messageBuffer, ClientSettings.ClientSession, mesId, seqNo, output);
        }
    }
}