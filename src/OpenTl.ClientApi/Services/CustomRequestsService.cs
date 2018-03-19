namespace OpenTl.ClientApi.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.MtProto.Interfaces;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;

    [SingleInstance(typeof(ICustomRequestsService))]
    public class CustomRequestsService : ICustomRequestsService
    {
        public IRequestSender RequestSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public IContextGetter ContextGetter { get; set; }

        /// <inheritdoc />
        public Task<TResult> SendRequestAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RequestSender.SendRequestAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, Func<Task<TResult>> requestFunc, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var requestExportAuthorization = new RequestExportAuthorization
                                             {
                                                 DcId = dcId
                                             };
            var exportedAuth = (TExportedAuthorization)await RequestSender.SendRequestAsync(requestExportAuthorization, cancellationToken).ConfigureAwait(false);

            var authKey = ClientSettings.ClientSession.AuthKey;
            var timeOffset = ClientSettings.ClientSession.TimeOffset;
            var serverAddress = ClientSettings.ClientSession.ServerAddress;
            var serverPort = ClientSettings.ClientSession.Port;

            var dc = ClientSettings.Config.DcOptions.Items.First(d => d.Id == dcId);
            ClientSettings.ClientSession.ServerAddress = dc.IpAddress;
            ClientSettings.ClientSession.Port = dc.Port;
            ClientSettings.ClientSession.AuthKey = null;
            ClientSettings.Config = null;

            await ContextGetter.Context.DisconnectAsync();

            var requestImportAuthorization = new RequestImportAuthorization
                                             {
                                                 Bytes = exportedAuth.Bytes,
                                                 Id = exportedAuth.Id
                                             };
            await SendRequestAsync(requestImportAuthorization, cancellationToken).ConfigureAwait(false);

            var result = await requestFunc();

            ClientSettings.ClientSession.AuthKey = authKey;
            ClientSettings.ClientSession.TimeOffset = timeOffset;
            ClientSettings.ClientSession.ServerAddress = serverAddress;
            ClientSettings.ClientSession.Port = serverPort;
            ClientSettings.Config = null;

            await ContextGetter.Context.DisconnectAsync();

            return result;
        }
        
        /// <inheritdoc />
        public async Task<TResult> SendRequestToOtherDcAsync<TResult>(int dcId, IRequest<TResult> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendRequestToOtherDcAsync(dcId, async () => await SendRequestAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}