namespace OpenTl.ClientApi
{
    using NullGuard;

    using OpenTl.ClientApi.Settings;

    public interface IFactorySettings
    {
        int AppId { get; set; }

        string AppHash { get; set; }

        string ServerAddress { get; set; }
        
        int ServerPort { get; set; }

        string ServerPublicKey { get; set; }
        
        string SessionTag { get; set; }

        ApplicationProperties Properties { get; set; }
        
        ISessionStore SessionStore { get; set; }
    }

    public sealed class FactorySettings : IFactorySettings
    {
        public int AppId { get; set; }

        public string AppHash { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string ServerPublicKey { get; set; }

        public string SessionTag { get; set; }

        public ApplicationProperties Properties { get; set; }

        [AllowNull]
        public ISessionStore SessionStore { get; set; }
    }
}