namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface IUsersService
    {
        /// <summary>
        /// Get full information about the user
        /// </summary>
        /// <param name="inputUser">User</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Full information</returns>
        Task<TUserFull> GetUserFull(IInputUser inputUser, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Get full information about current user
        /// </summary>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Full information</returns>
        Task<TUserFull> GetCurrentUserFull(CancellationToken cancellationToken = default(CancellationToken));
    }
}