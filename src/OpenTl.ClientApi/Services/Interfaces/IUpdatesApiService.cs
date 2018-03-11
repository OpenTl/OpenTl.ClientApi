namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Updates;

    public delegate Task UpdateHandler(IUpdates update);

    public interface IUpdatesService
    {
        /// <summary>Get current states of updates</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>State</returns>
        Task<IState> GetCurrentState(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get manual updates</summary>
        /// <param name="currentState">From the state obtained in the method <inheritdoc cref="GetCurrentState" /></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IDifference> GetUpdates(IState currentState, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get automatic updates</summary>
        event UpdateHandler RecieveUpdates;
    }
}