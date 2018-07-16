namespace OpenTl.ClientApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;



    using NullGuard;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.Extensions;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Messages;

    using IChatFull = OpenTl.Schema.Messages.IChatFull;

    [SingleInstance(typeof(IMessagesService))]
    internal class MessagesService : IMessagesService
    {
        private static readonly Random Random = new Random();

        public IClientSettings ClientSettings { get; set; }

        public IRequestSender RequestSender { get; set; }

        /// <inheritdoc />
        public async Task<IUpdates> AddChatUserAsync(int chatId, IInputUser user, int limit, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestAddChatUser
                          {
                              ChatId = chatId,
                              UserId = user,
                              FwdLimit = limit
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> CreateChatAsync(string title, IReadOnlyList<IInputUser> users, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestCreateChat
                          {
                              Title = title,
                              Users = new TVector<IInputUser>(users.ToArray())
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> DeleteChatUser(int chatId, IInputUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestDeleteChatUser
                          {
                              ChatId = chatId,
                              UserId = user
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IAffectedHistory> DeleteHistoryAsync(IInputPeer peer, int maxId, bool justClear, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var deleteHistory = new RequestDeleteHistory
                                {
                                    Peer = peer,
                                    JustClear = justClear,
                                    MaxId = maxId
                                };

            return await RequestSender.SendRequestAsync(deleteHistory, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IAffectedMessages> DeleteMessagesAsync(IReadOnlyList<int> ids, bool revoke, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var deleteMessages = new RequestDeleteMessages
                                 {
                                     Id = new TVector<int>(ids.ToArray()),
                                     Revoke = revoke
                                 };

            return await RequestSender.SendRequestAsync(deleteMessages, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> EditChatPhoto(int chatId, IInputChatPhoto photo, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestEditChatPhoto
                          {
                              ChatId = chatId,
                              Photo = photo
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> EditChatTitle(int chatId, string title, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestEditChatTitle
                          {
                              ChatId = chatId,
                              Title = title
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> ForwardMessagesAsync(IInputPeer fromPeer,
                                                         IInputPeer toPeer,
                                                         IReadOnlyList<int> ids,
                                                         bool silent,
                                                         bool withMyScore,
                                                         CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var forwardMessages = new RequestForwardMessages
                                  {
                                      FromPeer = fromPeer,
                                      ToPeer = toPeer,
                                      Id = new TVector<int>(ids.ToArray()),
                                      Background = false,
                                      Silent = silent,
                                      WithMyScore = withMyScore,
                                      RandomId = new TVector<long>(Random.NextLong())
                                  };

            return await RequestSender.SendRequestAsync(forwardMessages, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IChats> GetChatsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestGetChats { Id = new TVector<int>(ids.ToArray()) };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IChatFull> GetFullChatAsync(int chatId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestGetFullChat { ChatId = chatId };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IMessages> GetHistoryAsync(IInputPeer peer, int offset, int maxId, int limit, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestGetHistory
                      {
                          Peer = peer,
                          AddOffset = offset,
                          MaxId = maxId,
                          Limit = limit
                      };
            return await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IMessages> GetMessagesAsync(IReadOnlyList<IInputMessage> inputMessages, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var getMessagesRequest = new RequestGetMessages
                                     {
                                         Id = new TVector<IInputMessage>(inputMessages.ToArray())
                                     };

            return await RequestSender.SendRequestAsync(getMessagesRequest, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IDialogs> GetUserDialogsAsync(int limit = 100, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var getDialogs = new RequestGetDialogs
                             {
                                 OffsetDate = 0,
                                 OffsetPeer = new TInputPeerSelf(),
                                 Limit = limit
                             };

            return await RequestSender.SendRequestAsync(getDialogs, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IAffectedMessages> ReadHistoryAsync(IInputPeer peer, int maxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var readHistory = new RequestReadHistory
                              {
                                  Peer = peer,
                                  MaxId = maxId
                              };

            return await RequestSender.SendRequestAsync(readHistory, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IAffectedMessages> ReadMessageContentsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var readMessageContents = new RequestReadMessageContents
                                      {
                                          Id = new TVector<int>(ids.ToArray())
                                      };

            return await RequestSender.SendRequestAsync(readMessageContents, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IReceivedNotifyMessage>> ReceivedMessagesAsync(int maxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var receivedMessages = new RequestReceivedMessages
                                   {
                                       MaxId = maxId
                                   };

            var result = await RequestSender.SendRequestAsync(receivedMessages, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<IUpdates> SendMediaAsync(IInputPeer peer, IInputMedia media, string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var sendMedia = new RequestSendMedia
                            {
                                RandomId = Random.NextLong(),
                                Peer = peer,
                                Media = media,
                                Message = message,
                                Background = false,
                                ClearDraft = false
                            };

            return await RequestSender.SendRequestAsync(sendMedia, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> SendMessageAsync(IInputPeer peer, string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            return await RequestSender.SendRequestAsync(
                       new RequestSendMessage
                       {
                           Peer = peer,
                           Message = message,
                           RandomId = Random.NextLong()
                       },
                       cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> SendTypingAsync(IInputPeer peer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var req = new RequestSetTyping
                      {
                          Action = new TSendMessageTypingAction(),
                          Peer = peer
                      };

            return await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> SendUploadedDocumentAsync(IInputPeer peer,
                                                              IInputFile document,
                                                              string mimeType,
                                                              IReadOnlyList<IDocumentAttribute> attributes,
                                                              string message,
                                                              [AllowNull] IInputFile thumb = null,
                                                              CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestSendMedia
                          {
                              RandomId = Random.NextLong(),
                              Background = false,
                              ClearDraft = false,
                              Message = message,
                              Media = new TInputMediaUploadedDocument
                                      {
                                          File = document,
                                          MimeType = mimeType,
                                          Thumb = thumb,
                                          Attributes = new TVector<IDocumentAttribute>(attributes.ToArray())
                                      },
                              Peer = peer
                          };

            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IUpdates> SendUploadedPhotoAsync(IInputPeer peer, IInputFile photo, string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var request = new RequestSendMedia
                          {
                              RandomId = Random.NextLong(),
                              Background = false,
                              ClearDraft = false,
                              Message = message,
                              Media = new TInputMediaUploadedPhoto
                                      {
                                          File = photo,
                                      },
                              Peer = peer
                          };
            return await RequestSender.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}