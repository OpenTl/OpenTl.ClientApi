namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Handshake.Codecs
{
    using System;

    using DotNetty.Buffers;
    using DotNetty.Transport.Channels.Embedded;

    using OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Xunit;

    public class HandshakeRequestEncoderTest: UnitTest
    {
        private static readonly Random Random = new Random();
        
        [Fact]
        public void RequestReqPqMultyEndode()
        {
            this.RegisterType<HandshakeRequestEncoder>();

            // ---
            
            var requestEncoder = this.Resolve<HandshakeRequestEncoder>();
            
            var channel = new EmbeddedChannel(requestEncoder);

            var requestReqPqMulty = new RequestReqPqMulty
                                    {
                                        Nonce = new byte[16]
                                    };
            
            requestReqPqMulty.Nonce = new byte[16];
            Random.NextBytes(requestReqPqMulty.Nonce);

            channel.WriteOutbound(requestReqPqMulty);

            var data = channel.ReadOutbound<IByteBuffer>();
            
            // ---
            
            Assert.Equal(0, data.ReadLongLE());
            Assert.NotEqual(0, data.ReadLongLE());

            var lenght = data.ReadIntLE();
            Assert.Equal(lenght, data.ReadableBytes);

            var resultRequest = Serializer.Deserialize(data);
            
            Assert.IsType<RequestReqPqMulty>(resultRequest);

            Assert.Equal(requestReqPqMulty.Nonce, ((RequestReqPqMulty)resultRequest).Nonce);
        }
    }
}