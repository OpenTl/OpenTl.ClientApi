namespace OpenTl.ClientApi.Settings
{
    using System.Threading.Tasks;

    internal sealed class MemorySessionStore: ISessionStore
    {
        private byte[] _session;

        public void Dispose()
        {
        }

        public byte[] Load()
        {
            return _session;
        }

        public Task Save(byte[] session)
        {
            _session = session;
            
            return Task.CompletedTask;
        }

        public void SetSessionTag(string sessionTag)
        {
        }
    }
}