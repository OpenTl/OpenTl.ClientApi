namespace OpenTl.ClientApi.MtProto
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>LogOut service</summary>
    public interface ILogoutService
    {
        /// <summary>Logout current user</summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task Logout(CancellationToken cancellationToken);
    }
}