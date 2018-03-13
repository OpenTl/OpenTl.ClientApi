namespace OpenTl.ClientApi.MtProto
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface IRequestSender
    {
        /// <summary>
        /// Send custom requests
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TResult">Request result type</typeparam>
        /// <returns>Result</returns>
        Task<TResult> SendRequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken);
    }
}