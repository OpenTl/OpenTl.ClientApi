namespace OpenTl.ClientApi.Services
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using log4net;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.ClientApi.Settings;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Account;
    using OpenTl.Schema.Auth;

    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    [SingleInstance(typeof(IAuthService))]
    internal sealed class AuthService : IAuthService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AuthService));

        public IRequestSender RequestSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public ISessionWriter SessionWriter { get; set; }

        public ILogoutService LogoutService { get; set; }
        
        /// <inheritdoc />
        public int? CurrentUserId => ClientSettings.ClientSession.UserId;

        /// <inheritdoc />
        public async Task<TUser> CheckCloudPasswordAsync(string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(password, nameof(password)).IsNotNullOrWhiteSpace();

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var pwd = (TPassword)await RequestSender.SendRequestAsync(new RequestGetPassword(), cancellationToken).ConfigureAwait(false);
            var rv = pwd.CurrentSalt.Concat(passwordBytes).Concat(pwd.CurrentSalt);

            byte[] passwordHash;
            using (var sha = SHA256.Create())
            {
                passwordHash = sha.ComputeHash(rv.ToArray());
            }

            var request = new RequestCheckPassword
                          {
                              PasswordHash = passwordHash
                          };
            var result = (TAuthorization)await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.Is<TUser>();

            await OnUserAuthenticated(user).ConfigureAwait(false);

            return user;
        }

        /// <inheritdoc />
        public async Task<ISentCode> SendCodeAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();

            var request = new RequestSendCode
                          {
                              PhoneNumber = phoneNumber,
                              ApiId = ClientSettings.AppId,
                              ApiHash = ClientSettings.AppHash,
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task LogoutAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await LogoutService.Logout(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TUser> SignInAsync(string phoneNumber, ISentCode sentCode, string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();
            Guard.That(code, nameof(code)).IsNotNullOrWhiteSpace();

            var request = new RequestSignIn
                          {
                              PhoneNumber = phoneNumber,
                              PhoneCodeHash = sentCode.PhoneCodeHash,
                              PhoneCode = code
                          };

            var result = (TAuthorization)await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.Is<TUser>();

            await OnUserAuthenticated(user).ConfigureAwait(false);

            return user;
        }

        /// <inheritdoc />
        public async Task<TUser> SignUpAsync(string phoneNumber,
                                             ISentCode sentCode,
                                             string code,
                                             string firstName,
                                             string lastName,
                                             CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();
            Guard.That(code, nameof(code)).IsNotNullOrWhiteSpace();
            Guard.That(firstName, nameof(firstName)).IsNotNullOrWhiteSpace();
            Guard.That(lastName, nameof(lastName)).IsNotNullOrWhiteSpace();

            var request = new RequestSignUp
                          {
                              PhoneNumber = phoneNumber,
                              PhoneCode = code,
                              PhoneCodeHash = sentCode.PhoneCodeHash,
                              FirstName = firstName,
                              LastName = lastName
                          };
            var result = (TAuthorization)await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.Is<TUser>();

            await OnUserAuthenticated(user).ConfigureAwait(false);
            return user;
        }
        

        private async Task OnUserAuthenticated(TUser user)
        {
            var session = ClientSettings.ClientSession;

            session.UserId = user.Id;

            await SessionWriter.Save(session).ConfigureAwait(false);
        }
    }
}