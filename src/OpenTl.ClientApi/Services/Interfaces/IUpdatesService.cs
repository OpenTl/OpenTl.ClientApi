using System;

namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Updates;

    public delegate void UpdateHandler(IDifference update);

    public interface IUpdatesService
    {
        /// <summary>Get current states of updates</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>State</returns>
        Task<IState> GetCurrentStateAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get manual updates</summary>
        /// <param name="currentState">From the state obtained in the method <inheritdoc cref="GetCurrentStateAsync" /></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IDifference> GetUpdatesFromState(IState currentState, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starting receive updates. Updates will be raise an event <seealso cref="ReceiveUpdates"/>
        /// </summary>
        /// <param name="updatePeriod">Update period</param>
        void StartReceiveUpdates(TimeSpan updatePeriod);
        
        /// <summary>
        /// Stopping receive updates
        /// </summary>
        void StopReceiveUpdates();
        
        /// <summary>Get updates</summary>
        event UpdateHandler ReceiveUpdates;
    }
}