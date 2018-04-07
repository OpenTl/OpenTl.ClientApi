namespace OpenTl.ClientApi.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;

    [SingleInstance(typeof(IContactsService))]
    internal class ContactsService : IContactsService
    {
        public IRequestSender RequestSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        /// <inheritdoc />
        public async Task<TLink> DeleteContactAsync(IInputUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestDeleteContact
                      {
                          Id = user
                      };

            return (TLink)await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteContactsAsync(IReadOnlyList<IInputUser> users, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestDeleteContacts
                      {
                          Id = new TVector<IInputUser>(users.ToArray())
                      };

            return await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TContacts> GetContactsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestGetContacts { Hash = 0 };

            return (TContacts)await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TContactStatus>> GetStatusesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();
            var req = new RequestGetStatuses();

            var result = await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IImportedContacts> ImportContactsAsync(IReadOnlyList<TInputPhoneContact> contacts, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestImportContacts { Contacts = new TVector<IInputContact>(contacts.ToArray()) };

            return await RequestSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IFound> SearchUserAsync(string query, int limit = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var r = new RequestSearch
                    {
                        Q = query,
                        Limit = limit
                    };

            return await RequestSender.SendRequestAsync(r, cancellationToken).ConfigureAwait(false);
        }
    }
}