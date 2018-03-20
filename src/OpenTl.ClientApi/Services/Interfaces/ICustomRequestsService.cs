namespace OpenTl.ClientApi.Services.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface ICustomRequestsService
    {
        /// <summary>
        /// Send custom requests
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TResult">Request result type</typeparam>
        /// <returns>Result</returns>
        Task<TResult> SendRequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Send custom requests to the other DC
        /// </summary>
        /// <param name="dcId">DC id</param>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TResult">Request result type</typeparam>
        /// <returns>Result</returns>
        Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Send custom requests to the other DC
        /// </summary>
        /// <param name="dcId">DC id</param>
        /// <param name="requestFunc">Request function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TResult">Request result type</typeparam>
        /// <returns>Result</returns>
        Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, Func<IClientApi, Task<TResult>> requestFunc, CancellationToken cancellationToken = default(CancellationToken));
    }
}