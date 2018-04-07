namespace OpenTl.ClientApi.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using NullGuard;

    using OpenTl.ClientApi.Client.Interfaces;
    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;

    [SingleInstance(typeof(ICustomRequestsService))]
    internal class CustomRequestsService : ICustomRequestsService
    {
        public IRequestSender RequestSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public ITemploraryClientCache ClientCache { get; set; }
        
        /// <inheritdoc />
        [return:AllowNull]        
        public Task<TResult> SendRequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RequestSender.SendRequestAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        [return:AllowNull]        
        public async Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, Func<IClientApi, Task<TResult>> requestFunc, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var requestExportAuthorization = new RequestExportAuthorization
                                             {
                                                 DcId = dcId
                                             };
            var exportedAuth = (TExportedAuthorization)await RequestSender.SendRequestAsync(requestExportAuthorization, cancellationToken).ConfigureAwait(false);

            
            var dc = ClientSettings.Config.DcOptions.First(d => d.Id == dcId);

            var client = await ClientCache.GetOrCreate(ClientSettings, dc.IpAddress, dc.Port).ConfigureAwait(false);
            
            var requestImportAuthorization = new RequestImportAuthorization
                                             {
                                                 Bytes = exportedAuth.Bytes,
                                                 Id = exportedAuth.Id
                                             };

            await client.CustomRequestsService.SendRequestAsync(requestImportAuthorization, cancellationToken).ConfigureAwait(false);

            var result = await requestFunc(client).ConfigureAwait(false);

            return result;
        }
        
        /// <inheritdoc />
        public async Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendRequestToOtherDcAsync(dcId, async clienApi => await clienApi.CustomRequestsService.SendRequestAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}