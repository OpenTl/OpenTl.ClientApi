namespace OpenTl.ClientApi.MtProto
{
    using System.Threading.Tasks;

    public interface ISessionWriter
    {
        Task Save(IClientSession clientSession);
    }
}