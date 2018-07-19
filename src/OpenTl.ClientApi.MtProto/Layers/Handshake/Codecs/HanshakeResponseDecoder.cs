namespace OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs
{
    using System.Collections.Generic;

    using Castle.Windsor;

    using DotNetty.Buffers;
    using DotNetty.Codecs;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema.Serialization;

    [TransientInstance(typeof(IHandshakeHandler))]
    internal class HanshakeResponseDecoder: ByteToMessageDecoder, IHandshakeHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HanshakeResponseDecoder));

        public IWindsorContainer Container { get; set; }

        public IClientSettings ClientSettings { get; set; }
        
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input is EmptyByteBuffer)
            {
                return;
            }
            
            if (input.GetLongLE(0) != 0)
            {
                context.FireChannelRead(input.Retain());
                
                return;
            }

            input.SkipBytes(8);
            
            var messageId = input.ReadLongLE();
            input.SkipBytes(4);

            var message = Serializer.Deserialize(input);
        
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Receive the message {message} with id: {messageId}");
        
            output.Add(message);
        }
        
        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            Container.Release(this);
            
            base.ChannelInactive(ctx);
        }
    }
}