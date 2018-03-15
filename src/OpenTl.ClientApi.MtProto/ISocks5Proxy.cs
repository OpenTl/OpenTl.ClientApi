namespace OpenTl.ClientApi.MtProto
{
    using System.Net;

    /// <summary>SOCKS5 proxy configuration</summary>
    public interface ISocks5Proxy
    {
        /// <summary>Proxy-server address and port. <seealso cref="IPEndPoint"/> and <seealso cref="DnsEndPoint"/></summary>
        EndPoint Endpoint { get; set; }

        /// <summary>Username. Not required</summary>
        string Username { get; set; }

        /// <summary>Password. Not required</summary>
        string Password { get; set; }
    }
}