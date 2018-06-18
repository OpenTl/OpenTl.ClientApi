namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Top.Adapters
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoFixture;

    using DotNetty.Transport.Channels.Embedded;

    using Moq;

    using OpenTl.ClientApi.MtProto.Enums;
    using OpenTl.ClientApi.MtProto.Layers.Top.Adapters;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Auth;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;

    using Xunit;

    public sealed class TopHandlerAdapterTest: UnitTest
    {
        
        [Fact]
        public void InitfConnection_SaveConfig()
        {
            this.RegisterType<TopHandlerAdapter>();

            var mSettings =  this.BuildClientSettingsProps();

            var config = new TConfig
                         {
                             DcOptions = new TVector<IDcOption>(Fixture.Create<TDcOption>())
                         };
            
            this.Resolve<Mock<IRequestService>>()
                .BuildRegisterRequest<RequestInvokeWithLayer>(Task.FromResult((object)config));

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            // ---

            channel.Pipeline.FireUserEventTriggered(ESystemNotification.HandshakeComplete);

            // ---
            Assert.NotNull(channel.ReadOutbound<RequestInvokeWithLayer>());
            Assert.Equal(config, mSettings.Object.Config);
            
        }
        
        [Fact]
        public void SendAfterInit()
        {
            this.RegisterType<TopHandlerAdapter>();

            var mSettings = this.BuildClientSettingsProps();
            
            var authKey = mSettings.Object.ClientSession.AuthKey;
            
            var resultTaskSource = new TaskCompletionSource<object>();

            var request = new RequestPing();
            var response = new TPong();

            var config = new TConfig
                         {
                             DcOptions = new TVector<IDcOption>()
                         };
            
            this.Resolve<Mock<IRequestService>>()
                .BuildGetAllRequestToReply(request)
                .BuildRegisterRequest<RequestInvokeWithLayer>(Task.FromResult((object)config));
            
            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            // ---
            
            mSettings.Object.ClientSession.AuthKey = null;

            var channel = new EmbeddedChannel(handlerAdapter);

            mSettings.Object.ClientSession.AuthKey = authKey;

            channel.Pipeline.FireUserEventTriggered(ESystemNotification.HandshakeComplete);
            
            resultTaskSource.SetResult(response);
            
            // ---
            Assert.NotNull(channel.ReadOutbound<RequestInvokeWithLayer>());
            
            Assert.Equal(request, channel.ReadOutbound<RequestPing>());
        }
    }

}