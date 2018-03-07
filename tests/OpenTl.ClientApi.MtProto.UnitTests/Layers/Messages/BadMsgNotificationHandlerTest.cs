namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Messages
{
    using AutoFixture;

    using DotNetty.Transport.Channels.Embedded;

    using Moq;

    using OpenTl.ClientApi.MtProto.Layers.Messages.Adapters;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;

    using Xunit;

    public sealed class BadMsgNotificationHandlerTest : UnitTest
    {
        [Theory]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(32)]
        [InlineData(33)]
        [InlineData(34)]
        [InlineData(35)]
        [InlineData(48)]
        [InlineData(64)]
        public void HandleError(int errorCode)
        {
            this.RegisterType<BadMsgNotificationHandler>();

            var requestEncoder = this.Resolve<BadMsgNotificationHandler>();

            var channel = new EmbeddedChannel(requestEncoder);

            var messageId = Fixture.Create<long>();

            var badServerSalt = new TBadMsgNotification
                                {
                                    BadMsgId = messageId,
                                    BadMsgSeqno = Fixture.Create<int>(),
                                    ErrorCode = errorCode
                                };

            var request = new RequestPing();

            badServerSalt.ErrorCode = errorCode;

            var mRequestService = this.Resolve<Mock<IRequestService>>();
            mRequestService.BuildGetRequestToReply(messageId, request);

            // ---

            channel.WriteInbound(badServerSalt);

            // ---

            Assert.Equal(request, channel.ReadOutbound<RequestPing>());
        }
    }
}