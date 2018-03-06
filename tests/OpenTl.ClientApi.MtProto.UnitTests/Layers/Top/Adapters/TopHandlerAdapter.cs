namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Top.Adapters
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoFixture;

    using DotNetty.Transport.Channels.Embedded;

    using Moq;

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

            var resultTaskSource = new TaskCompletionSource<object>();
            
            var mRequestService = new Mock<IRequestService>();
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => resultTaskSource.Task);
                           
            this.RegisterMock(mRequestService);

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            var request = new RequestPing();

            // ---

            var resultTask = handlerAdapter.SendRequest(request);

            resultTaskSource.SetResult(true);
            
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
                
            var mRequestService = new Mock<IRequestService>();
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<RequestInvokeWithLayer>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => Task.FromResult((object)config));
                           
            this.RegisterMock(mRequestService);

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            var request = new RequestPing();

            // ---

            channel.WriteInbound(new TDhGenOk());

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
            
            var mRequestService = new Mock<IRequestService>();
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<RequestPing>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => resultTaskSource.Task);
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<RequestInvokeWithLayer>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => Task.FromResult((object)new TConfig()));
                           
            this.RegisterMock(mRequestService);

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            var request = new RequestPing();
            var response = new TPong();

            // ---

            var resultTask = handlerAdapter.SendRequest(request);

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

            this.BuildClientSettingsProps();
            
            var resultTaskSource = new TaskCompletionSource<object>();
            
            var mRequestService = new Mock<IRequestService>();
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<RequestPing>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => resultTaskSource.Task);
            mRequestService.Setup(rs => rs.RegisterRequest(It.IsAny<RequestInvokeWithLayer>(), It.IsAny<CancellationToken>()))
                           .Returns<IRequest, CancellationToken>((_, __) => Task.FromResult((object)new TConfig()));
                           
            this.RegisterMock(mRequestService);

            var handlerAdapter = this.Resolve<TopHandlerAdapter>();

            var channel = new EmbeddedChannel(handlerAdapter);

            var request = new RequestPing();
            var response = new TPong();

            // ---

            var resultTask = handlerAdapter.SendRequest(request);

            channel.WriteInbound(new TDhGenOk());

            resultTaskSource.SetResult(response);
            
            // ---
            Assert.NotNull(channel.ReadOutbound<RequestInvokeWithLayer>());
            
            Assert.Equal(request, channel.ReadOutbound<RequestPing>());

            Assert.Equal(response, resultTask.Result);
        }
    }

}