namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    using OpenTl.Schema;

    internal sealed class TestSettings: IClientSettings
    {
        public int AppId { get; set; }

        public string AppHash { get; set; }

        public string PublicKey { get; set; }

        public bool UseIPv6 { get; set; }

        public IApplicationProperties ApplicationProperties { get; set; } = new TestApplicationProperties();

        public ISocks5Proxy Socks5Proxy { get; set; }

        public IClientSession ClientSession { get; set; } = new TestSession();

        public IConfig Config { get; set; }
    }
}