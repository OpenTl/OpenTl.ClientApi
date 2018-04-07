namespace OpenTl.ClientApi.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Account;
    using OpenTl.Schema.Photos;
    using OpenTl.Schema.Users;

    [SingleInstance(typeof(IUsersService))]
    internal class UsersService : IUsersService
    {
        public IRequestSender RequestSender { get; set; }

        /// <inheritdoc />
        public Task<TUserFull> GetCurrentUserFullAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetUserFullAsync(new TInputUserSelf(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TUserFull> GetUserFullAsync(IInputUser inputUser, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestGetFullUser
                          {
                              Id = inputUser
                          };

            return (TUserFull)await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IPhotos> GetUserPhotosAsync(IInputUser user, int offset = 0, int limit = 200, long maxId = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestGetUserPhotos
                          {
                              UserId = user,
                              MaxId = maxId,
                              Limit = limit,
                              Offset = offset
                          };
            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ICollection<IUser>> GetUsersAsync(IReadOnlyList<IInputUser> inputUsers, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestGetUsers
                          {
                              Id = new TVector<IInputUser>(inputUsers.ToArray())
                          };
            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
        
        /// <inheritdoc />
        public async Task<bool> CheckUsernameAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestCheckUsername
                          {
                             Username = username
                          };
            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
        
        /// <inheritdoc />
        public async Task<TUser> UpdateUsernameAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestUpdateUsername
                          {
                              Username = username
                          };
            return (TUser) await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}