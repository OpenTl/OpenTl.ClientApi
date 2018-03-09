namespace OpenTl.ClientApi
{
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;

    using TelegramClient.Core.ApiServies.Interfaces;

    public interface ITelegramClient
    {
        /// <summary>Send a custom requests</summary>
        IPackageSender CustomRequestsService { get; }

        // // /// <summary>Automatic and manual updates</summary>
        // IUpdatesApiService UpdatesService { get; }

        // // /// <summary>Connecting to the server</summary>
        // IConnectApiService ConnectService { get; }

        /// <summary>Registration and authentication</summary>
        IAuthService AuthService { get; }

        /// <summary>Messages and chats</summary>
        IMessagesService MessagesService { get; }

        /// <summary>Working with contacts</summary>
        IContactsService ContactsService { get; }

        // // /// <summary>Working with files</summary>
        // IUploadApiService UploadService { get; }
    }
}