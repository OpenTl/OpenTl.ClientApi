namespace OpenTl.ClientApi.Client
{
    using System;
    using System.Threading;

    using Castle.Windsor;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    /// <inheritdoc />
    [SingleInstance(typeof(IClientApi))]
    internal class ClientApi : IClientApi
    {
        private Timer _keepAliveTimer;

        public IWindsorContainer Container { get; set; }

        /// <inheritdoc />
        public IUpdatesService UpdatesService { get; set; }

        /// <inheritdoc />
        public ICustomRequestsService CustomRequestsService { get; set; }

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

        /// <inheritdoc />
        public IHelpService HelpService { get; set; }

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