namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    using System.Threading.Tasks;

    internal sealed class SessionWriter: ISessionWriter
    {
        public Task Save(IClientSession clientSession) => Task.CompletedTask;
    }
}