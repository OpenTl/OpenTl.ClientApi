namespace OpenTl.ClientApi.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Help;

    /// <inheritdoc />
    [SingleInstance(typeof(IHelpService))]
    internal class HelpService : IHelpService
    {
        public IRequestSender RequestSender { get; set; }

        /// <inheritdoc />
        public async Task<TConfig> GetConfig(CancellationToken cancellationToken = default(CancellationToken))
        {
            return (TConfig)await RequestSender.SendRequestAsync(new RequestGetConfig(), cancellationToken).ConfigureAwait(false);
        }
    }
}