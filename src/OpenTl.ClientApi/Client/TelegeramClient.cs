namespace OpenTl.ClientApi.Client
{
    using Castle.Windsor;

    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;

    using TelegramClient.Core.ApiServies.Interfaces;

    [SingleInstance(typeof(ITelegramClient))]
    internal class TelegeramClient : ITelegramClient
    {
        public IWindsorContainer Container { get; set; }
        
        // public ISenderService SendService { get; set; }

       // // public IUpdatesApiService UpdatesService { get; set; }

       // // public IConnectApiService ConnectService { get; set; }

        /// <inheritdoc />
        public IAuthService AuthService { get; set; }

        /// <inheritdoc />
        public IContactsService ContactsService { get; set;  }

        // // public IMessagesApiService MessagesService { get; set; }

       // // public IUploadApiService UploadService { get; set; }

    }
}