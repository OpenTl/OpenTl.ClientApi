namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Photos;

    public interface IUsersService
    {
        /// <summary>Get full information about current user</summary>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Full information</returns>
        Task<TUserFull> GetCurrentUserFullAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get full information about the user</summary>
        /// <param name="inputUser">User</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Full information</returns>
        Task<TUserFull> GetUserFullAsync(IInputUser inputUser, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get a user photos</summary>
        /// <param name="user">User</param>
        /// <param name="offset">Offset</param>
        /// <param name="limit">Limit</param>
        /// <param name="maxId">Max id</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Users</returns>
        Task<IPhotos> GetUserPhotosAsync(IInputUser user, int offset = 0, int limit = 200, long maxId = 0, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get users</summary>
        /// <param name="inputUsers">Users for information</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Users</returns>
        Task<ICollection<IUser>> GetUsersAsync(IReadOnlyList<IInputUser> inputUsers, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Check username</summary>
        /// <param name="username">Username</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Is available</returns>
        Task<bool> CheckUsernameAsync(string username, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Update username</summary>
        /// <param name="username">Username</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>User</returns>
        Task<TUser> UpdateUsernameAsync(string username, CancellationToken cancellationToken = default(CancellationToken));
    }
}