namespace OpenTl.ClientApi
{
    using System.Net;

    using NullGuard;

    using OpenTl.ClientApi.MtProto;
    
    /// <inheritdoc />
    public sealed class Socks5ProxyConfig: ISocks5Proxy
    {
        /// <inheritdoc />
        public EndPoint Endpoint { get; set; }

        /// <inheritdoc />
        [AllowNull]
        public string Username { get; set; }

        /// <inheritdoc />
        [AllowNull]
        public string Password { get; set; }
    }
}