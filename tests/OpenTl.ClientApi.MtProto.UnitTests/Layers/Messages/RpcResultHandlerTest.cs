namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Messages
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using AutoFixture;

    using DotNetty.Buffers;
    using DotNetty.Transport.Channels.Embedded;

    using Moq;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Layers.Messages.Adapters;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Extesions;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Xunit;

    public sealed class RpcResultHandlerTest : UnitTest
    {
        [Fact]
        public void ReturnResult()
        {
            this.RegisterType<RpcResultHandler>();

            var messageId = Fixture.Create<long>();

            var result = new TPong();

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = result
                          };

            var mRequestService = this.Resolve<Mock<IRequestService>>()
                                      .BuildReturnResult(messageId, result);

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);
            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            mRequestService.Verify(service => service.ReturnResult(messageId, result), Times.Once);
        }
        
        [Fact]
        public void ReturnZippedResult()
        {
            this.RegisterType<RpcResultHandler>();

            var messageId = Fixture.Create<long>();

            
            var result = new TPong();

            var tgZipPacked = new TgZipPacked
                              {
                                  PackedData = Fixture.CreateMany<byte>(8).ToArray()
                              };
            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = tgZipPacked
                          };

            this.Resolve<Mock<IUnzippedService>>()
                .Setup(service => service.UnzipPackage(tgZipPacked))
                .Returns(() => result);

            var mRequestService = this.Resolve<Mock<IRequestService>>()
                                      .BuildReturnResult(messageId, result);

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);
            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            mRequestService.Verify(service => service.ReturnResult(messageId, result), Times.Once);
        }
        
        [Theory]
        [InlineData("SESSION_PASSWORD_NEEDED", typeof(CloudPasswordNeededException))]
        [InlineData("PHONE_CODE_INVALID", typeof(PhoneCodeInvalidException))]
        [InlineData("FILE_MIGRATE_1", typeof(FileMigrationException))]
        [InlineData("FLOOD_WAIT_1000", typeof(FloodWaitException))]
        [InlineData("SOME_EXCEPTION", typeof(InvalidOperationException))]
        public void ReturnError_SimpleCases(string errorMessage, Type exceptionType)
        {
            this.RegisterType<RpcResultHandler>();

            var messageId = Fixture.Create<long>();

            var result = new TRpcError
                         {
                             ErrorMessage = errorMessage
                         };

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = result
                          };

            var mRequestService = this.Resolve<Mock<IRequestService>>()
                                      .BuildReturnException(messageId, exceptionType);

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);
            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            mRequestService.Verify(service => service.ReturnException(messageId, It.Is<Exception>(ex => ex.GetType() == exceptionType)), Times.Once);
        }
        
        [Theory]
        [InlineData("PHONE_MIGRATE_")]
        [InlineData("USER_MIGRATE_")]
        [InlineData("NETWORK_MIGRATE_")]
        public void ReturnError_Migration(string errorMessage)
        {
            this.RegisterType<RpcResultHandler>();

           var mSettings = this.BuildClientSettingsProps();

            var dcOption = mSettings.Object.Config.DcOptions.Items.First();
            errorMessage += dcOption.Id;
            
            var messageId = Fixture.Create<long>();

            var result = new TRpcError
                         {
                             ErrorMessage = errorMessage
                         };

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = result
                          };

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);
            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            Assert.False(channel.Open);
            
            Assert.Equal(dcOption.IpAddress, mSettings.Object.ClientSession.ServerAddress);
            Assert.Equal(dcOption.Port, mSettings.Object.ClientSession.Port);
            Assert.Null(mSettings.Object.ClientSession.AuthKey);
        }
    }
}