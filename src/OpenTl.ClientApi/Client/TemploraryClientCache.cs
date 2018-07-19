namespace OpenTl.ClientApi.Client
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;

    using OpenTl.ClientApi.Client.Interfaces;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(ITemploraryClientCache))]
    internal class TemploraryClientCache : ITemploraryClientCache
    {
        private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(10);

        public async Task<IClientApi> GetOrCreate(IClientSettings clientSettings, string ipAddress, int port)
        {
            var serverAddress = $"{ipAddress}:{port}";
            
            if (_memoryCache.TryGetValue(serverAddress, out IClientApi clientApi))
            {
                return clientApi;
            }

            clientApi = await ClientFactory.BuildTempClientAsync(clientSettings, ipAddress, port).ConfigureAwait(false);
            
            _memoryCache.Set(serverAddress, clientApi, new MemoryCacheEntryOptions().SetSlidingExpiration(SlidingExpiration));
            
            return clientApi;
        }        
    }
}