using OpenTl.Schema;

namespace OpenTl.ClientApi.MtProto
{
    /// <summary>
    ///     Handle auto updates
    /// </summary>
    public interface IAutoUpdatesHandler
    {
        void HandleAutoUpdates(IUpdates update);
    }
}