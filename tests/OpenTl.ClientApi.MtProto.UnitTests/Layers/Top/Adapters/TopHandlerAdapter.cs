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
        public void WaitForInitizalize()
        {
            this.RegisterType<TopHandlerAdapter>();

            var mSettings =  this.BuildClientSettingsProps();
            mSettings.Object.ClientSession.AuthKey = null;
            
            var resultTaskSource = new TaskCompletionSource<object>();
            
            var request = new RequestPing();

            this.Resolve<Mock<IRequestService>>()
                .BuildRegisterRequest<RequestPing>(resultTaskSource.Task);

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            // ---

            var resultTask = handlerAdapter.SendRequestAsync(request, CancellationToken.None);
            
            // ---

            Assert.Null(channel.ReadOutbound<object>());
            
            Assert.Equal(TaskStatus.WaitingForActivation, resultTask.Status);
        }
        
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
        public void InitWithExistSession()
        {
            this.RegisterType<TopHandlerAdapter>();

            var msession = this.BuildClientSettingsProps();

            msession.Object.ClientSession.AuthKey = new AuthKey(Fixture.CreateMany<byte>(256).ToArray());
            var resultTaskSource = new TaskCompletionSource<object>();
            
            this.Resolve<Mock<IRequestService>>()
                .BuildRegisterRequest<RequestPing>(resultTaskSource.Task)
                .BuildRegisterRequest<RequestInvokeWithLayer>(Task.FromResult((object)new TConfig()));

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            var request = new RequestPing();
            var response = new TPong();

            // ---

            var resultTask = handlerAdapter.SendRequestAsync(request, CancellationToken.None);

            resultTaskSource.SetResult(response);
            
            // ---
            Assert.NotNull(channel.ReadOutbound<RequestInvokeWithLayer>());
            
            Assert.Equal(request, channel.ReadOutbound<RequestPing>());

            Assert.Equal(response, resultTask.Result);
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
            
            this.Resolve<Mock<IRequestService>>()
                .BuildRegisterRequest<RequestPing>(resultTaskSource.Task)
                .BuildGetAllRequestToReply(request)
                .BuildRegisterRequest<RequestInvokeWithLayer>(Task.FromResult((object)new TConfig()));
            
            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            // ---
            
            mSettings.Object.ClientSession.AuthKey = null;

            var channel = new EmbeddedChannel(handlerAdapter);

            mSettings.Object.ClientSession.AuthKey = authKey;

            channel.Pipeline.FireUserEventTriggered(ESystemNotification.HandshakeComplete);
            
            var resultTask = handlerAdapter.SendRequestAsync(request, CancellationToken.None);
            
            resultTaskSource.SetResult(response);
            
            // ---
            Assert.NotNull(channel.ReadOutbound<RequestInvokeWithLayer>());
            
            Assert.Equal(request, channel.ReadOutbound<RequestPing>());

            Assert.Equal(response, resultTask.Result);
        }
    }

}