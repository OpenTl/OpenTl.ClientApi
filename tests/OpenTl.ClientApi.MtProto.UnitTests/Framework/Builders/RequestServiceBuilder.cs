namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using System;
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
        
        public static Mock<IRequestService> BuildReturnResult(this Mock<IRequestService> mock, long messageId, object result)
        {
            mock.Setup(service => service.ReturnResult(messageId, result));
            
            return mock;
        }
        
        public static Mock<IRequestService> BuildReturnException<TException>(this Mock<IRequestService> mock, long messageId) where TException: Exception
        {
            return BuildReturnException(mock, messageId, typeof(TException));
        }
        
        public static Mock<IRequestService> BuildReturnException(this Mock<IRequestService> mock, long messageId, Type exceptionType)
        {
            mock.Setup(service => service.ReturnException(messageId, It.Is<Exception>(ex => ex.GetType() == exceptionType)));
            
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