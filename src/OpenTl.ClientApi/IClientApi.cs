namespace OpenTl.ClientApi
{
    using System;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;

    /// <summary>Entry point to client API MtProto protocol</summary>
    public interface IClientApi : IDisposable
    {
        /// <summary>Send a custom requests</summary>
        ICustomRequestsService CustomRequestsService { get; }

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

        /// <summary>Help requests</summary>
        IHelpService HelpService { get; set; }
    }
}