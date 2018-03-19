namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    /// <summary>Help requests</summary>
    public interface IHelpService
    {
        /// <summary>Get current config</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Config</returns>
        Task<TConfig> GetConfig(CancellationToken cancellationToken = default(CancellationToken));
    }
}