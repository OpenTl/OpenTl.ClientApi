namespace OpenTl.ClientApi.MtProto
{
    using OpenTl.Schema;

    public interface IClientSettings
    {
        int AppId { get; set; }

        string AppHash { get; set; }

        string PublicKey { get; set; }

        bool UseIPv6 { get; set; }

        IApplicationProperties ApplicationProperties {get; set; }

        ISocks5Proxy Socks5Proxy { get; set; }
        
        IClientSession ClientSession { get; set; }
        
        IConfig Config { get; set; }
    }
}