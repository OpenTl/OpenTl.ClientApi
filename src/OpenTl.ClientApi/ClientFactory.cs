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

    public static class ClientFactory
    {
        public static async Task<IClientApi> BuildClient(IFactorySettings factorySettings)
        {
            var container = WindsorFactory.Create(typeof(INettyBootstrapper).GetTypeInfo().Assembly, typeof(IClientApi).GetTypeInfo().Assembly);

            FillSettings(container, factorySettings);

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