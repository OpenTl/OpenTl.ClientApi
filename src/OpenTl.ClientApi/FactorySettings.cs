namespace OpenTl.ClientApi
{
    using NullGuard;

    /// <summary>Factory settings are required to build the client</summary>
    public interface IFactorySettings
    {
        /// <summary>Application Id. You can take here - <a href="https://my.telegram.org/apps">Telegram</a></summary>
        int AppId { get; set; }

        /// <summary>Application hash. You can take here - <a href="https://my.telegram.org/apps">Telegram</a></summary>
        string AppHash { get; set; }

        /// <summary>Connection server address</summary>
        string ServerAddress { get; set; }

        /// <summary>Connection server port</summary>
        int ServerPort { get; set; }

        /// <summary>Server public RSA key. You can take here - <a href="https://my.telegram.org/apps">Telegram</a></summary>
        string ServerPublicKey { get; set; }

        /// <summary>Session tag. By default is "session"</summary>
        string SessionTag { get; set; }

        /// <summary>Use IP v6 protocol. Defaults: True</summary>
        bool UseIPv6 { get; set; }

        /// <summary>Application properties</summary>
        ApplicationProperties Properties { get; set; }

        /// <summary>SOCKS5 proxy configuration. Not required</summary>
        Socks5ProxyConfig ProxyConfig { get; set; }

        /// <summary>Session store. Not required</summary>
        ISessionStore SessionStore { get; set; }
    }

    /// <inheritdoc />
    public sealed class FactorySettings : IFactorySettings
    {
        /// <inheritdoc />
        public int AppId { get; set; }

        /// <inheritdoc />
        public string AppHash { get; set; }

        /// <inheritdoc />
        public string ServerAddress { get; set; }

        /// <inheritdoc />
        public int ServerPort { get; set; }

        /// <inheritdoc />
        public string ServerPublicKey { get; set; }

        /// <inheritdoc />
        public string SessionTag { get; set; }

        /// <inheritdoc />
        public bool UseIPv6 { get; set; } = true;

        /// <inheritdoc />
        public ApplicationProperties Properties { get; set; }

        /// <inheritdoc />
        [AllowNull]
        public Socks5ProxyConfig ProxyConfig { get; set; }

        /// <inheritdoc />
        [AllowNull]
        public ISessionStore SessionStore { get; set; }
    }
}