namespace OpenTl.ClientApi
{
    using System;
    using System.Threading;

    using Castle.Windsor;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    /// <summary>Entry point to client API MtProto protocol</summary>
    public interface IClientApi : IDisposable
    {
        /// <summary>Send a custom requests</summary>
        IRequestSender CustomRequestsService { get; }

        /// <summary>Automatic and manual updates</summary>
        IUpdatesService UpdatesService { get; }

        /// <summary>Registration and authentication</summary>
        IAuthService AuthService { get; }

        /// <summary>Messages and chats</summary>
        IMessagesService MessagesService { get; }

        /// <summary>Working with contacts</summary>
        IContactsService ContactsService { get; }

        /// <summary>Working with users</summary>
        IUsersService UsersService { get; }

        /// <summary>Working with files</summary>
        IFileService FileService { get; }

        /// <summary>
        ///     The server closes the connection if the client does not send requests for some time. This method sends an
        ///     inquiry to the server every hour and keeps the connection open
        /// </summary>
        void KeepAliveConnection();
    }

    [SingleInstance(typeof(IClientApi))]
    internal class ClientApi : IClientApi
    {
        private Timer _keepAliveTimer;

        public IWindsorContainer Container { get; set; }

        public IUpdatesService UpdatesService { get; set; }

        /// <inheritdoc />
        public IRequestSender CustomRequestsService { get; set; }

        /// <inheritdoc />
        public IAuthService AuthService { get; set; }

        /// <inheritdoc />
        public IContactsService ContactsService { get; set; }

        /// <inheritdoc />
        public IUsersService UsersService { get; set; }

        /// <inheritdoc />
        public IMessagesService MessagesService { get; set; }

        /// <inheritdoc />
        public IFileService FileService { get; set; }

        public void Dispose()
        {
            _keepAliveTimer?.Dispose();
            Container?.Dispose();
        }

        /// <inheritdoc />
        public void KeepAliveConnection()
        {
            _keepAliveTimer = new Timer(
                _ =>
                {
                    var requestPing = new RequestPing { PingId = new Random().NextLong() };

                    CustomRequestsService.SendRequestAsync(requestPing, CancellationToken.None).ConfigureAwait(false);
                },
                null,
                TimeSpan.FromHours(1),
                TimeSpan.FromHours(0));
        }
    }
}