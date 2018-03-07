namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Schema;

    internal static class RequestServiceBuilder
    {
        public static Mock<IRequestService> BuildRegisterRequest<TRequest>(this Mock<IRequestService> mock, Task<object> task) where  TRequest: IRequest
        {
            mock
                .Setup(service => service.RegisterRequest(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => task);

            return mock;
        }
        
        public static Mock<IRequestService> BuildGetAllRequestToReply(this Mock<IRequestService> mock, params IRequest[] requests)
        {
            mock
                .Setup(service => service.GetAllRequestToReply())
                .Returns(() => requests);

            return mock;
        }
    }
}