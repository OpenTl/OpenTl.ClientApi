namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    using OpenTl.Schema;

    public class TestSettings: IClientSettings
    {
        public int AppId { get; set; }

        public string AppHash { get; set; }

        public string PublicKey { get; set; }

        public IApplicationProperties ApplicationProperties { get; set; } = new TestApplicationProperties();

        public IClientSession ClientSession { get; set; } = new TestSession();

        public IConfig Config { get; set; }
    }
}