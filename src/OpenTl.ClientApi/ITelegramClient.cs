namespace OpenTl.ClientApi
{
 using OpenTl.ClientApi.Services.Interfaces;

 public interface ITelegramClient
    {
        // /// <summary>Send custom messages</summary>
        // ISenderService SendService { get; }

       // // /// <summary>Automatic and manual updates</summary>
        // IUpdatesApiService UpdatesService { get; }

       // // /// <summary>Connecting to the server</summary>
        // IConnectApiService ConnectService { get; }

        /// <summary>Registration and authentication</summary>
        IAuthService AuthService { get; }

        // /// <summary>Messages and chats</summary>
        // IMessagesApiService MessagesService { get; }

       // // /// <summary>Working with contacts</summary>
        // IContactsApiService ContactsService { get; }

       // // /// <summary>Working with files</summary>
        // IUploadApiService UploadService { get; }
    }
}