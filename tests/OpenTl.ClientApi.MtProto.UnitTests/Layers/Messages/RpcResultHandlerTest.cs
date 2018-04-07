namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Messages
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoFixture;

    using DotNetty.Transport.Channels.Embedded;

    using Moq;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Layers.Messages.Adapters;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Extensions;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Xunit;

    public sealed class RpcResultHandlerTest : UnitTest
    {
        [Theory]
        [InlineData("PHONE_MIGRATE_")]
        [InlineData("USER_MIGRATE_")]
        [InlineData("NETWORK_MIGRATE_")]
        public async Task ReturnError_Migration(string errorMessage)
        {
            this.BuildClientSettingsProps();
            this.RegisterType<RpcResultHandler>();

            var mSettings = this.BuildClientSettingsProps();

            this.Resolve<Mock<ISessionWriter>>()
                .BuildSuccessSave();

            var dcOption = mSettings.Object.Config.DcOptions.First();

            var messageId = Fixture.Create<long>();

            var result = new TRpcError
                         {
                             ErrorMessage = errorMessage + dcOption.Id
                         };

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = Serializer.Serialize(result).ToArray()
                          };

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteInbound(request);

            await Task.Delay(500);
            
            // ---

            Assert.IsType<TMsgsAck>(channel.ReadOutbound<TMsgsAck>());

            Assert.False(channel.Open);

            Assert.Equal(dcOption.IpAddress, mSettings.Object.ClientSession.ServerAddress);
            Assert.Equal(dcOption.Port, mSettings.Object.ClientSession.Port);
            Assert.Null(mSettings.Object.ClientSession.AuthKey);
        }

        [Theory]
        [InlineData("SESSION_PASSWORD_NEEDED", typeof(CloudPasswordNeededException))]
        [InlineData("PHONE_CODE_INVALID", typeof(PhoneCodeInvalidException))]
        [InlineData("FILE_MIGRATE_1", typeof(FileMigrationException))]
        [InlineData("FLOOD_WAIT_1000", typeof(FloodWaitException))]
        [InlineData("SOME_EXCEPTION", typeof(UnhandledException))]
        public void ReturnError_SimpleCases(string errorMessage, Type exceptionType)
        {
            this.BuildClientSettingsProps();
            
            this.RegisterType<RpcResultHandler>();

            var messageId = Fixture.Create<long>();

            var result = new TRpcError
                         {
                             ErrorMessage = errorMessage
                         };

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = Serializer.Serialize(result).ToArray()
                          };

            var mRequestService = this.Resolve<Mock<IRequestService>>()
                                      .BuildReturnException(messageId, exceptionType);

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            channel.Flush();
            Assert.IsType<TMsgsAck>(channel.ReadOutbound<TMsgsAck>());

            mRequestService.Verify(service => service.ReturnException(messageId, It.Is<Exception>(ex => ex.GetType() == exceptionType)), Times.Once);
        }

        [Fact]
        public void ReturnResult()
        {
            this.BuildClientSettingsProps();
            
            this.RegisterType<RpcResultHandler>();

            var messageId = Fixture.Create<long>();

            var result = new TPong();

            var request = new TRpcResult
                          {
                              ReqMsgId = messageId,
                              Result = Serializer.Serialize(result).ToArray()
                          };

            var mRequestService = this.Resolve<Mock<IRequestService>>()
                                      .BuildReturnResult(messageId, result);

            var requestEncoder = this.Resolve<RpcResultHandler>();

            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteInbound(request);

            // ---

            Assert.Null(channel.ReadOutbound<object>());

            channel.Flush();
            Assert.IsType<TMsgsAck>(channel.ReadOutbound<TMsgsAck>());

            mRequestService.Verify(service => service.ReturnResult(messageId, result), Times.Once);
        }

        [Fact]
        public void ReturnZippedResult()
        {
            this.BuildClientSettingsProps();
            
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
                              Result = Serializer.Serialize(tgZipPacked).ToArray()
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

            channel.Flush();
            Assert.IsType<TMsgsAck>(channel.ReadOutbound<TMsgsAck>());

            mRequestService.Verify(service => service.ReturnResult(messageId, result), Times.Once);
        }
    }
}