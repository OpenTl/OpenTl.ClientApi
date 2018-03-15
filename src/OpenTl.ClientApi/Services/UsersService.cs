namespace OpenTl.ClientApi.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Users;

    [SingleInstance(typeof(IUsersService))]
    public class UsersService : IUsersService
    {
        public IRequestSender RequestSender { get; set; }

        /// <inheritdoc />
        public async Task<TUserFull> GetUserFull(IInputUser inputUser, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestGetFullUser
                          {
                              Id = inputUser
                          };

            return (TUserFull) await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<TUserFull> GetCurrentUserFull(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetUserFull(new TInputUserSelf(), cancellationToken);
        }
    }
}