namespace OpenTl.ClientApi.Services
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Account;
    using OpenTl.Schema.Auth;

    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    [SingleInstance(typeof(IAuthService))]
    internal sealed class AuthService : IAuthService
    {
        public IPackageSender PackageSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public ISessionWriter SessionWriter { get; set; }
        
        public long? CurrentUserId => ClientSettings.ClientSession.UserId;

        public async Task<IPassword> GetPasswordSetting(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await PackageSender.SendRequestAsync(new RequestGetPassword(), cancellationToken).ConfigureAwait(false);
        }

        public async Task<ICheckedPhone> IsPhoneRegisteredAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();

            var authCheckPhoneRequest = new RequestCheckPhone
                                        {
                                            PhoneNumber = phoneNumber
                                        };
            return await PackageSender.SendRequestAsync(authCheckPhoneRequest, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TUser> MakeAuthAsync(string phoneNumber, string phoneCodeHash, string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();
            Guard.That(phoneCodeHash, nameof(phoneCodeHash)).IsNotNullOrWhiteSpace();
            Guard.That(code, nameof(code)).IsNotNullOrWhiteSpace();

            var request = new RequestSignIn
                          {
                              PhoneNumber = phoneNumber,
                              PhoneCodeHash = phoneCodeHash,
                              PhoneCode = code
                          };

            var result = (TAuthorization)await PackageSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.Cast<TUser>();

            await OnUserAuthenticated(user).ConfigureAwait(false);

            return user;
        }

        public async Task<TUser> MakeAuthWithPasswordAsync(TPassword password, string passwordStr, CancellationToken cancellationToken = default(CancellationToken))
        {
            var passwordBytes = Encoding.UTF8.GetBytes(passwordStr);
            var rv = password.CurrentSalt.Concat(passwordBytes).Concat(password.CurrentSalt);

            byte[] passwordHash;
            using (var sha = SHA256.Create())
            {
                passwordHash = sha.ComputeHash(rv.ToArray());
            }

            var request = new RequestCheckPassword
                          {
                              PasswordHash = passwordHash
                          };
            var result = (TAuthorization)await PackageSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.As<TUser>();

            await OnUserAuthenticated(user).ConfigureAwait(false);

            return user;
        }

        public async Task<ISentCode> SendCodeRequestAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            Guard.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrWhiteSpace();

            var request = new RequestSendCode
                          {
                              PhoneNumber = phoneNumber,
                              ApiId = ClientSettings.AppId,
                              ApiHash = ClientSettings.AppHash
                          };
            return await PackageSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TUser> SignUpAsync(string phoneNumber, string phoneCodeHash, string code, string firstName, string lastName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new RequestSignUp
                          {
                              PhoneNumber = phoneNumber,
                              PhoneCode = code,
                              PhoneCodeHash = phoneCodeHash,
                              FirstName = firstName,
                              LastName = lastName
                          };
            var result = (TAuthorization)await PackageSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

            var user = result.User.Cast<TUser>();

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