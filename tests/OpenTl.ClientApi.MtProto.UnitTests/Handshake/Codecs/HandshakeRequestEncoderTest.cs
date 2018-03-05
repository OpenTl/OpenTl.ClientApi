namespace OpenTl.ClientApi.MtProto.UnitTests.Handshake.Codecs
{
    using System;

    using DotNetty.Buffers;
    using DotNetty.Transport.Channels.Embedded;

    using OpenTl.ClientApi.MtProto.Layers.Handshake.Codecs;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Xunit;

    public class HandshakeRequestEncoderTest: BaseTest
    {
        private static readonly Random Random = new Random();
        
        [Fact]
        public void RequestReqPqMultyEndode()
        {
            this.RegisterType<HandshakeRequestEncoder>();

            var mSession = SessionMock.Create();
            mSession
               .Setup(session => session.GenerateMsgId())
               .Returns(() => 1);

            var settings = ClientSettingsMock.Create()
                              .AttachSession(() => mSession.Object);
            
            this.RegisterMock(settings);

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
            Assert.Equal(1, data.ReadLongLE());

            var lenght = data.ReadIntLE();
            Assert.Equal(lenght, data.ReadableBytes);

            var resultRequest = Serializer.Deserialize(data);
            
            Assert.IsType<RequestReqPqMulty>(resultRequest);

            Assert.Equal(requestReqPqMulty.Nonce, ((RequestReqPqMulty)resultRequest).Nonce);
        }
    }
}