namespace OpenTl.ClientApi.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Updates;

    [SingleInstance(typeof(IUpdatesService), typeof(IUpdatesRaiser))]
    internal class UpdatesService : IUpdatesService,
                                       IUpdatesRaiser
    {
        public IClientSettings ClientSettings { get; set; }

        public IRequestSender SenderService { get; set; }

        /// <inheritdoc />
        public async Task<IState> GetCurrentStateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            return await SenderService.SendRequestAsync(new RequestGetState(), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IDifference> GetUpdatesFromState(IState currentState, CancellationToken cancellationToken = default(CancellationToken))
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