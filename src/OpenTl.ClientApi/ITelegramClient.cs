namespace OpenTl.ClientApi
{
    using System;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;

    using TelegramClient.Core.ApiServies.Interfaces;

    public interface ITelegramClient: IDisposable
    {
        /// <summary>Send a custom requests</summary>
        IPackageSender CustomRequestsService { get; }

        /// <summary>Automatic and manual updates</summary>
        IUpdatesService UpdatesService { get; }

        /// <summary>Registration and authentication</summary>
        IAuthService AuthService { get; }

        /// <summary>Messages and chats</summary>
        IMessagesService MessagesService { get; }

        /// <summary>Working with contacts</summary>
        IContactsService ContactsService { get; }

        /// <summary>
        /// The server closes the connection if the client does not send requests for some time.
        /// This method sends an inquiry to the server every hour and keeps the connection open
        /// </summary>
        void KeepAliveConnection();

        // // /// <summary>Working with files</summary>
        // IUploadApiService UploadService { get; }
    }
}