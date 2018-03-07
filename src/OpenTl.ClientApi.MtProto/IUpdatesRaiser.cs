namespace OpenTl.ClientApi.MtProto
{
    using OpenTl.Schema;

    public interface IUpdatesRaiser
    {
        void OnUpdateRecieve(IUpdates msg);
    }
}