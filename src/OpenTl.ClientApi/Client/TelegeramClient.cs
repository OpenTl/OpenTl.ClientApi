namespace OpenTl.ClientApi.Client
{
    using Castle.Windsor;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(ITelegramClient))]
    internal class TelegeramClient : ITelegramClient
    {
        public IWindsorContainer Container { get; set; }
        
        // public ISenderService SendService { get; set; }

       // // public IUpdatesApiService UpdatesService { get; set; }

       // // public IConnectApiService ConnectService { get; set; }

        public IAuthService AuthService { get; set; }

       // // public IMessagesApiService MessagesService { get; set; }

       // // public IContactsApiService ContactsService { get; set; }

       // // public IUploadApiService UploadService { get; set; }

    }
}