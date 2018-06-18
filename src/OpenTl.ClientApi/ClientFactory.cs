namespace OpenTl.ClientApi
{
    using System.Reflection;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Settings;
    using OpenTl.Common.IoC;

    /// <summary>Entry point to working with the library</summary>
    public static class ClientFactory
    {
        /// <summary>Build the client</summary>
        /// <param name="factorySettings">Settings</param>
        /// <returns>Client</returns>
        public static async Task<IClientApi> BuildClientAsync(IFactorySettings factorySettings)
        {
            var container = WindsorFactory.Create(typeof(INettyBootstrapper).GetTypeInfo().Assembly, typeof(IClientApi).GetTypeInfo().Assembly);

            FillSettings(container, factorySettings);

            var bootstrapper = container.Resolve<INettyBootstrapper>();
            await bootstrapper.Init();

            return container.Resolve<IClientApi>();
        }

        /// <summary>Build the client from existing settings</summary>
        /// <param name="clientSettings">Settings</param>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="port">Port</param>
        /// <returns>Client</returns>
        internal static async Task<IClientApi> BuildTempClientAsync(IClientSettings clientSettings, string ipAddress, int port)
        {
            var container = WindsorFactory.Create(typeof(INettyBootstrapper).GetTypeInfo().Assembly, typeof(IClientApi).GetTypeInfo().Assembly);

            container.Register(Component.For<ISessionStore>().ImplementedBy<MemorySessionStore>().IsDefault());
            
            var settings = container.Resolve<IClientSettings>();
            settings.AppHash = clientSettings.AppHash;
            settings.AppId = clientSettings.AppId;
            settings.ApplicationProperties = clientSettings.ApplicationProperties;
            settings.PublicKey = clientSettings.PublicKey;
            settings.Socks5Proxy = clientSettings.Socks5Proxy;
            settings.ClientSession = clientSettings.ClientSession.Clone();
            
            settings.ClientSession.Port = port;
            settings.ClientSession.ServerAddress = ipAddress;
            
            var bootstrapper = container.Resolve<INettyBootstrapper>();
            await bootstrapper.Init();

            return container.Resolve<IClientApi>();
        }

        private static void FillSettings(IWindsorContainer container, IFactorySettings factorySettings)
        {
            Guard.That(factorySettings.AppId).IsPositive();
            Guard.That(factorySettings.AppHash).IsNotNullOrWhiteSpace();
            Guard.That(factorySettings.ServerAddress).IsNotNullOrWhiteSpace();
            Guard.That(factorySettings.ServerPublicKey).IsNotNullOrWhiteSpace();
            Guard.That(factorySettings.ServerPort).IsPositive();

            var settings = container.Resolve<IClientSettings>();

            settings.AppId = factorySettings.AppId;
            settings.AppHash = factorySettings.AppHash;
            settings.PublicKey = factorySettings.ServerPublicKey;

            settings.ApplicationProperties = factorySettings.Properties;
            settings.Socks5Proxy = factorySettings.ProxyConfig;
            settings.UseIPv6 = factorySettings.UseIPv6;

            if (factorySettings.SessionStore != null)
            {
                container.Register(Component.For<ISessionStore>().Instance(factorySettings.SessionStore).IsDefault());
            }

            var sessionStore = container.Resolve<ISessionStore>();
            sessionStore.SetSessionTag(factorySettings.SessionTag);

            settings.ClientSession = TryLoadOrCreateNew(sessionStore, factorySettings);
        }

        private static IClientSession TryLoadOrCreateNew(ISessionStore sessionStore, IFactorySettings factorySettings)
        {
            var newSession = new ClientSession();

            var sessionData = sessionStore.Load();
            if (sessionData != null)
            {
                newSession.FromBytes(sessionData);
            }
            else
            {
                newSession.ServerAddress = factorySettings.ServerAddress;
                newSession.Port = factorySettings.ServerPort;
            }

            return newSession;
        }
    }
}