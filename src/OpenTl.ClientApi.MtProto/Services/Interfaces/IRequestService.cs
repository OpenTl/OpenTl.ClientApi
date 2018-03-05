namespace OpenTl.ClientApi.MtProto.Services.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    internal interface IRequestService
    {
        Task<object> RegisterRequest(IRequest request, CancellationToken cancellationToken);
        
        void SetMessageId(IRequest request, long messageId);

        void ReturnException(long messageId, Exception exception);

        void ReturnException(Exception exception);

        void ReturnResult(long messageId, object obj);
    }
}