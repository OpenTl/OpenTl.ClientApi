using System;
using log4net;
using OpenTl.Schema;

namespace OpenTl.ClientApi.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema.Updates;

    [SingleInstance(typeof(IUpdatesService), typeof(IAutoUpdatesHandler))]
    internal class UpdatesService : IUpdatesService, IAutoUpdatesHandler, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UpdatesService));

        public IClientSettings ClientSettings { get; set; }

        public IRequestSender SenderService { get; set; }

        private Timer _updateTimer;

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
        public void StartReceiveUpdates(TimeSpan updatePeriod)
        {
            _updateTimer?.Dispose();
            
            _updateTimer = new Timer(async _ =>
                {
                    var state = ClientSettings.ClientSession.UpdateState;
                    if (state != null)
                    {
                        var diff = await GetUpdatesFromState(state).ConfigureAwait(false);
                        await HandleManualsUpdates(diff).ConfigureAwait(false);
                    }
                    else
                    {
                        ClientSettings.ClientSession.UpdateState = (TState) await GetCurrentStateAsync().ConfigureAwait(false);
                    }
                },
                null,
                TimeSpan.FromSeconds(0),
                updatePeriod);
        }

        /// <inheritdoc />
        public void StopReceiveUpdates()
        {
            _updateTimer?.Dispose();
            _updateTimer = null;
        }
        
        /// <inheritdoc />
        public event ManualUpdateHandler ManualReceiveUpdates;

        /// <inheritdoc />
        public event AutoUpdateHandler AutoReceiveUpdates;

        public void Dispose()
        {
            _updateTimer?.Dispose();
        }
        
        private async Task HandleManualsUpdates(IDifference diff)
        {
            switch (diff)
            {
                case TDifference difference:
                    ClientSettings.ClientSession.UpdateState = (TState) difference.State;
                    
                    ManualReceiveUpdates?.Invoke(difference);
                    break;
                case TDifferenceEmpty _:
                    return;
                case TDifferenceSlice differenceSlice:
                    var newState = ClientSettings.ClientSession.UpdateState = (TState) differenceSlice.IntermediateState;
                    
                    ManualReceiveUpdates?.Invoke(differenceSlice);

                    await GetUpdatesFromState(newState).ConfigureAwait(false);
                    
                    break;
                case TDifferenceTooLong differenceTooLong:
                    ManualReceiveUpdates?.Invoke(differenceTooLong);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(diff));
            }
        }

        public void HandleAutoUpdates(IUpdates update)
        {
            try
            {
                AutoReceiveUpdates?.Invoke(update);
            }
            catch (Exception e)
            {
                Log.Error("Error processing updates", e);
            }
        }
    }
}