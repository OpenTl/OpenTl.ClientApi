namespace OpenTl.ClientApi.Settings
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(ISessionWriter))]
    internal class SessionWriter : ISessionWriter
    {
        public ISessionStore SessionStore { get; set; }

        public async Task Save(IClientSession clientSession)
        {
            await SessionStore.Save(clientSession.ToBytes());
        }
    }
}