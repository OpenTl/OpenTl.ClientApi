namespace OpenTl.ClientApi.MtProto
{
    using System.Threading.Tasks;

    using OpenTl.Schema;

    public interface IUpdatesRaiser
    {
        Task OnUpdateRecieve(IUpdates msg);
    }
}