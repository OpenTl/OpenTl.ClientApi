namespace OpenTl.ClientApi.Client.Interfaces
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto;

    internal interface ITemploraryClientCache
    {
        Task<IClientApi> GetOrCreate(IClientSettings clientSettings, string ipAddress, int port);
    }
}