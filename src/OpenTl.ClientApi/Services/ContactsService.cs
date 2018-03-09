namespace OpenTl.ClientApi.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;

    using TelegramClient.Core.ApiServies.Interfaces;

    [SingleInstance(typeof(IContactsService))]
    internal class ContactsService : IContactsService
    {
        public IPackageSender PackageSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        /// <inheritdoc />
        public async Task<TContacts> GetContactsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var req = new RequestGetContacts { Hash = 0 };

            return (TContacts) await PackageSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<TContactStatus>> GetStatusesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();
            var req = new RequestGetStatuses();

           var result = await PackageSender.SendRequestAsync(req, cancellationToken).ConfigureAwait(false);

            return result.Items;
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

            return await PackageSender.SendRequestAsync(r, cancellationToken).ConfigureAwait(false);
        }
    }
    
}