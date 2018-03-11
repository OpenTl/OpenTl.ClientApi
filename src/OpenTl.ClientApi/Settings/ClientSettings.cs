namespace OpenTl.ClientApi.Settings
{
    using NullGuard;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IClientSettings))]
    internal class ClientSettings : IClientSettings
    {
        public int AppId { get; set; }

        public string AppHash { get; set; }

        public string PublicKey { get; set; }

        public IApplicationProperties ApplicationProperties { get; set; }

        public IClientSession ClientSession { get; set; }

        [AllowNull]
        public IConfig Config { get; set; }
    }
}