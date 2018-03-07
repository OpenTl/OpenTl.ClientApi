namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Messages
{
    using System;

    using DotNetty.Transport.Channels.Embedded;

    using Moq;

    using OpenTl.ClientApi.MtProto.Layers.Messages.Adapters;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;

    using Xunit;

    public sealed class UpdatesHandlerTest : UnitTest
    {
        [Theory]
        [InlineData(typeof(TUpdateShort))]
        [InlineData(typeof(TUpdateShortChatMessage))]
        [InlineData(typeof(TUpdateShortMessage))]
        [InlineData(typeof(TUpdateShortSentMessage))]
        [InlineData(typeof(TUpdates))]
        [InlineData(typeof(TUpdatesCombined))]
        [InlineData(typeof(TUpdatesTooLong))]
        public void BadServerSaltHandle(Type messageType)
        {
            this.RegisterType<UpdatesHandler>();

            var requestEncoder = this.Resolve<UpdatesHandler>();

            var update = (IUpdates) Activator.CreateInstance(messageType);

            var mUpdatesRaiser = this.Resolve<Mock<IUpdatesRaiser>>();
            mUpdatesRaiser.Setup(m => m.OnUpdateRecieve(update));
            
            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteInbound(update);

            // ---

            mUpdatesRaiser.Verify(m => m.OnUpdateRecieve(update), Times.Once);
        }
    }
}