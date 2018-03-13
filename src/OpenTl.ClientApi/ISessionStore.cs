namespace OpenTl.ClientApi
{
    using System;
    using System.Threading.Tasks;

    public interface ISessionStore : IDisposable
    {
        byte[] Load();

        Task Remove();

        Task Save(byte[] session);

        void SetSessionTag(string sessionTag);
    }
}