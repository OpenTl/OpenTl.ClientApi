namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Messages;

    using IChatFull = OpenTl.Schema.Messages.IChatFull;

    public interface IMessagesService
    {
        /// <summary>Adds a user to a chat and sends a service message on it.</summary>
        /// <param name="chatId">Chat's identifier</param>
        /// <param name="user">User ID to be added</param>
        /// <param name="limit">Number of last messages to be forwarded</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        ///     Returns a <see cref="IUpdates" /> object contains info on one message with auxiliary data and data on the
        ///     current state of updates.
        /// </returns>
        Task<IUpdates> AddChatUserAsync(int chatId, IInputUser user, int limit, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Creates a new chat.</summary>
        /// <param name="title">Chat name</param>
        /// <param name="users">List of user IDs to be invited</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during an action.</returns>
        Task<IUpdates> CreateChatAsync(string title, IReadOnlyList<IInputUser> users, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Deletes a user from a chat and sends a service message on it.</summary>
        /// <param name="chatId">Chat's identifier</param>
        /// <param name="user">User ID to be deleted</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during the action.</returns>
        Task<IUpdates> DeleteChatUser(int chatId, IInputUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Deletes communication history.</summary>
        /// <param name="peer">User or chat, communication history of which will be deleted</param>
        /// <param name="maxId">
        ///     If a positive value was transferred, the method will return only messages with IDs less than the
        ///     set one
        /// </param>
        /// <param name="justClear">Delete as non-recoverable or just clear the history</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IAffectedHistory" /> object containing a affected history</returns>
        Task<IAffectedHistory> DeleteHistoryAsync(IInputPeer peer, int maxId, bool justClear, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Deletes messages by their IDs.</summary>
        /// <param name="ids">Identifiers of messages</param>
        /// <param name="revoke">Delete messages for everyone</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IAffectedMessages" /> object containing a affected messages</returns>
        Task<IAffectedMessages> DeleteMessagesAsync(IReadOnlyList<int> ids, bool revoke, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Changes chat photo and sends a service message on it.</summary>
        /// <param name="chatId">Chat's identifier</param>
        /// <param name="photo">Photo to be set</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during an action.</returns>
        Task<IUpdates> EditChatPhoto(int chatId, IInputChatPhoto photo, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Chanages chat name and sends a service message on it.</summary>
        /// <param name="chatId">Chat's identifier</param>
        /// <param name="title">New chat name, different from the old one</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during an action.</returns>
        Task<IUpdates> EditChatTitle(int chatId, string title, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Forwards messages by their IDs.</summary>
        /// <param name="fromPeer">User or chat from where a message will be forwarded</param>
        /// <param name="toPeer">User or chat where a message will be forwarded</param>
        /// <param name="ids">Forwarded message IDs</param>
        /// <param name="silent"></param>
        /// <param name="withMyScore"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during an action.</returns>
        Task<IUpdates> ForwardMessagesAsync(IInputPeer fromPeer,
                                            IInputPeer toPeer,
                                            IReadOnlyList<int> ids,
                                            bool silent,
                                            bool withMyScore,
                                            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Returns chat basic info on their IDs.</summary>
        /// <param name="ids">Identifiers of chats</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object contains list of chats with auxiliary data.</returns>
        Task<IChats> GetChatsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Returns full chat info according to its ID.</summary>
        /// <param name="chatId">Chat's identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object contains extended info on chat with auxiliary data.</returns>
        Task<IChatFull> GetFullChatAsync(int chatId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Returns messages from the history of a single conversation</summary>
        /// <param name="peer">Target user or group</param>
        /// <param name="offset">Number of list elements to be skipped</param>
        /// <param name="maxId">
        ///     If a positive value was transferred, the method will return only messages with IDs less than
        ///     <inheritdoc cref="maxId" />
        /// </param>
        /// <param name="limit">Number of list elements to be returned</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>History messages</returns>
        Task<IMessages> GetHistoryAsync(IInputPeer peer, int offset, int maxId, int limit, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Returns the list of messages by their IDs.</summary>
        /// <param name="inputMessages">messages</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Object contains list of messages</returns>
        Task<IMessages> GetMessagesAsync(IReadOnlyList<IInputMessage> inputMessages, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Returns a list of the current user’s conversations</summary>
        /// <param name="limit">Number of list elements to be returned</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Dialogs</returns>
        Task<IDialogs> GetUserDialogsAsync(int limit = 100, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Marks message history as read.</summary>
        /// <param name="peer">User or group to receive the message</param>
        /// <param name="maxId">
        ///     If a positive value is passed, only messages with identifiers less or equal than the given one will
        ///     be read
        /// </param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IAffectedMessages" /> object containing a affected messages</returns>
        Task<IAffectedMessages> ReadHistoryAsync(IInputPeer peer, int maxId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Notifies the sender about the recipient having listened a voice message or watched a video.</summary>
        /// <param name="ids">Identifiers of messages</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IAffectedMessages" /> object containing a affected messages</returns>
        Task<IAffectedMessages> ReadMessageContentsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Confirms that a client has received messages and cancels push notifications</summary>
        /// <param name="maxId">Maximum message ID available in a client.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Notifications</returns>
        Task<IReadOnlyList<IReceivedNotifyMessage>> ReceivedMessagesAsync(int maxId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Sends a non-text message.</summary>
        /// <param name="peer">User or group to receive the message</param>
        /// <param name="media">Content</param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Returns a <see cref="IUpdates" /> object containing a service message sent during the action.</returns>
        Task<IUpdates> SendMediaAsync(IInputPeer peer, IInputMedia media, string message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Sends a text message.</summary>
        /// <param name="peer">User or chat where a message will be sent</param>
        /// <param name="message">Message text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IUpdates> SendMessageAsync(IInputPeer peer, string message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Transmits the current user typing event to the conversation partner or group</summary>
        /// <param name="peer">Target user or group</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<bool> SendTypingAsync(IInputPeer peer, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Sends a document with <inheritdoc cref="mimeType" /> to user or chat</summary>
        /// <param name="peer">User or chat where a message will be sent</param>
        /// <param name="document">Document</param>
        /// <param name="mimeType">MimeType</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="message">Message</param>
        /// <param name="thumb">The thumb</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updates</returns>
        Task<IUpdates> SendUploadedDocumentAsync(IInputPeer peer,
                                                 IInputFile document,
                                                 string mimeType,
                                                 IReadOnlyList<IDocumentAttribute> attributes,
                                                 string message,
                                                 IInputFile thumb = null,
                                                 CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Sends a photo to user or chat</summary>
        /// <param name="peer">User or chat where a message will be sent</param>
        /// <param name="photo">Photo</param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updates</returns>
        Task<IUpdates> SendUploadedPhotoAsync(IInputPeer peer, IInputFile photo, string message, CancellationToken cancellationToken = default(CancellationToken));
    }
}