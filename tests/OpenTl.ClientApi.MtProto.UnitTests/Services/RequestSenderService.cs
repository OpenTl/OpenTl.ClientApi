namespace OpenTl.ClientApi.MtProto.UnitTests.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoFixture;

    using DotNetty.Transport.Channels;

    using Moq;

    using OpenTl.ClientApi.MtProto.Interfaces;
    using OpenTl.ClientApi.MtProto.Services;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Auth;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;

    using Xunit;

    public sealed class RequestSenderServiceTest: UnitTest
    {
        [Fact]
        public void WaitForInitizalize()
        {
            this.RegisterType<RequestSenderService>();

            var mSettings =  this.BuildClientSettingsProps();
            mSettings.Object.ClientSession.AuthKey = null;

            var mContextGetter = this.Resolve<Mock<IContextGetter>>();
            
            var resultTaskSource = new TaskCompletionSource<object>();
            
            var request = new RequestPing();

            this.Resolve<Mock<IRequestService>>()
                .BuildRegisterRequest<RequestPing>(resultTaskSource.Task);

            var senderService = this.Resolve<RequestSenderService>();

            // ---

            var resultTask = senderService.SendRequestAsync(request, CancellationToken.None);
            
            // ---

            Assert.Equal(TaskStatus.WaitingForActivation, resultTask.Status);
            mContextGetter.Verify(getter => getter.Context, Times.Never);
        }
        
        [Fact]
        public async Task SendAfterInit()
        {
            this.RegisterType<RequestSenderService>();

            ContextGetterBuilder.Build(Container);
            
            var msession = this.BuildClientSettingsProps();
            msession.Object.ClientSession.AuthKey = new AuthKey(Fixture.CreateMany<byte>(256).ToArray());

            var mContext= this.Resolve<Mock<IChannelHandlerContext>>();

            var mRequestService = this.Resolve<Mock<IRequestService>>();
            var requestPing = new RequestPing();
            
            //--
            
            var requestSender = this.Resolve<RequestSenderService>();
            await requestSender.SendRequestAsync(requestPing, CancellationToken.None);
            
            //--
            
            mContext.Verify(context => context.WriteAndFlushAsync(requestPing), Times.Once);
            mRequestService.Verify(context => context.RegisterRequest(requestPing, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}