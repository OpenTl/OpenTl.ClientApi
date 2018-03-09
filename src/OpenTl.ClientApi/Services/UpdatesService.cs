namespace OpenTl.ClientApi.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Updates;

    using TelegramClient.Core.ApiServies.Interfaces;

    [SingleInstance(typeof(IUpdatesService), typeof(IUpdatesRaiser))]
    internal class UpdatesApiService : IUpdatesService,
                                       IUpdatesRaiser
    {

        public IClientSettings ClientSettings { get; set; }
        
        public IPackageSender SenderService { get; set; }

        /// <inheritdoc />
        public async Task<IState> GetCurrentState(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();
            
            return await SenderService.SendRequestAsync(new RequestGetState(), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IDifference> GetUpdates(IState currentState, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            var getDiffRequest = new RequestGetDifference
                                 {
                                     Pts = currentState.Pts,
                                     Qts = currentState.Qts,
                                     Date = currentState.Date
                                 };

            return await SenderService.SendRequestAsync(getDiffRequest, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task OnUpdateRecieve(IUpdates message)
        {
            if (RecieveUpdates != null)
            {
                await RecieveUpdates.Invoke(message).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public event UpdateHandler RecieveUpdates;
    }
}