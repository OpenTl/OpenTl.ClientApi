namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;

    /// <summary>Registration and authentication</summary>
    public interface IAuthService
    {
        /// <summary>Returns the current user ID. Null - if the session is not authenticated</summary>
        int? CurrentUserId { get; }

        /// <summary>
        ///     Sends a cloud password if an exception <inheritdoc cref="CloudPasswordNeededException" /> is caught in the
        ///     previous step <inheritdoc cref="SignInAsync" />
        /// </summary>
        /// <param name="password"></param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns></returns>
        Task<TUser> CheckCloudPasswordAsync(string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Logout the current user</summary>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Logout succeess</returns>
        Task LogoutAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>The first stage of authentication or registration. Request to send a verification code</summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns>Information about the sent code</returns>
        Task<ISentCode> SendCodeAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>The second stage of authentication. Send the received authentication code</summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="sentCode">
        ///     The object <inheritdoc cref="ISentCode" /> obtained in the previous step
        ///     <inheritdoc cref="SendCodeAsync" />>
        /// </param>
        /// <param name="code">Received authentication code</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns></returns>
        Task<TUser> SignInAsync(string phoneNumber, ISentCode sentCode, string code, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Register new user</summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="sentCode">
        ///     The object <inheritdoc cref="ISentCode" /> obtained in the previous step
        ///     <inheritdoc cref="SendCodeAsync" />>
        /// </param>
        /// <param name="code">Received authentication code</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="cancellationToken">Сancellation token</param>
        /// <returns></returns>
        Task<TUser> SignUpAsync(string phoneNumber,
                                ISentCode sentCode,
                                string code,
                                string firstName,
                                string lastName,
                                CancellationToken cancellationToken = default(CancellationToken));
    }
}