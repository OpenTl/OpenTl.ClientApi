namespace OpenTl.ClientApi.Client
{
    using System;
    using System.Threading;

    using Castle.Windsor;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    using TelegramClient.Core.ApiServies.Interfaces;

    [SingleInstance(typeof(ITelegramClient))]
    internal class TelegeramClient : ITelegramClient
    {
        private Timer _keepAliveTimer;

        public IWindsorContainer Container { get; set; }

        // public ISenderService SendService { get; set; }

        // // public IUpdatesApiService UpdatesService { get; set; }

        // // public IConnectApiService ConnectService { get; set; }

        /// <inheritdoc />
        public IPackageSender CustomRequestsService { get; set; }

        /// <inheritdoc />
        public IAuthService AuthService { get; set; }

        /// <inheritdoc />
        public IContactsService ContactsService { get; set; }

        /// <inheritdoc />
        public IMessagesService MessagesService { get; set; }

        // // public IUploadApiService UploadService { get; set; }

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