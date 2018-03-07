namespace OpenTl.ClientApi.MtProto
{
    using System.Threading.Tasks;

    public interface INettyBootstrapper
    {
        Task Init();

        Task Connect();
    }
}