namespace OpenTl.ClientApi.MtProto.Layers.Secure.Codecs
{
    using System.Collections.Generic;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Common.MtProto;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(ISecureHandler))]
    internal sealed class SecureRequestDecoder: ByteToMessageDecoder,
                                         ISecureHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecureRequestDecoder));

        public IClientSettings ClientSettings { get; set; }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var decodeBuffer = MtProtoHelper.FromServerDecrypt(input, ClientSettings.ClientSession, out var authKeyId, out var serverSalt, out var sessionId, out var messageId, out var seqNumber);

            var message = Serializer.Deserialize(decodeBuffer);

            Log.Debug($"Recieve the secure message {message}");

            output.Add(message);
        }
    }
}