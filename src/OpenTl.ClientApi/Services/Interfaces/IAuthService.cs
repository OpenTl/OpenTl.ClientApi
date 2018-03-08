namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Account;
    using OpenTl.Schema.Auth;

    public interface IAuthService
    {
        long? CurrentUserId { get; }
        
        Task<IPassword> GetPasswordSetting(CancellationToken cancellationToken = default(CancellationToken));

        Task<ICheckedPhone> IsPhoneRegisteredAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<TUser> MakeAuthAsync(string phoneNumber, string phoneCodeHash, string code, CancellationToken cancellationToken = default(CancellationToken));

        Task<TUser> MakeAuthWithPasswordAsync(TPassword password, string passwordStr, CancellationToken cancellationToken = default(CancellationToken));

        Task<ISentCode> SendCodeRequestAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<TUser> SignUpAsync(string phoneNumber, string phoneCodeHash, string code, string firstName, string lastName, CancellationToken cancellationToken = default(CancellationToken));
    }
}