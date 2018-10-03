using System;
using OpenTl.Schema;

namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema.Updates;

    public delegate void ManualUpdateHandler(IDifference update);
    
    public delegate void AutoUpdateHandler(IUpdates update);

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
        /// Starting receive updates. Updates will be raise an event <seealso cref="ManualReceiveUpdates"/>
        /// </summary>
        /// <param name="updatePeriod">Update period</param>
        void StartReceiveUpdates(TimeSpan updatePeriod);
        
        /// <summary>
        /// Stopping receive updates
        /// </summary>
        void StopReceiveUpdates();
        
        /// <summary>Get manuals updates</summary>
        event ManualUpdateHandler ManualReceiveUpdates;
        
        /// <summary>Get auto updates</summary>
        event AutoUpdateHandler AutoReceiveUpdates;
    }
}