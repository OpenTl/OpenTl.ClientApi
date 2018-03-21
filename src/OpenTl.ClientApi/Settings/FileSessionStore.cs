namespace OpenTl.ClientApi.Settings
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using log4net;

    using NullGuard;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(ISessionStore))]
    internal sealed class FileSessionStore : ISessionStore
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileSessionStore));

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private string _sessionFile;

        public void Dispose()
        {
            _semaphore?.Dispose();
        }

        [return: AllowNull]
        public byte[] Load()
        {
            Log.Debug($"Load session for sessionTag = {_sessionFile}");

            return File.Exists(_sessionFile)
                       ? File.ReadAllBytes(_sessionFile)
                       : null;
        }

        public Task Remove()
        {
            if (File.Exists(_sessionFile))
            {
                File.Delete(_sessionFile);
            }

            return Task.FromResult(true);
        }

        public async Task Save(byte[] session)
        {
            Log.Debug($"Save session into {_sessionFile}");

            await _semaphore.WaitAsync().ConfigureAwait(false);

            File.WriteAllBytes(_sessionFile, session);

            _semaphore.Release();
        }

        public void SetSessionTag(string sessionTag)
        {
            _sessionFile = Path.Combine(System.AppContext.BaseDirectory, $"{sessionTag}.dat");
        }
    }
}