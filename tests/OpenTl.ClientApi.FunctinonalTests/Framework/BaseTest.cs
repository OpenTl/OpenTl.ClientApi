namespace OpenTl.ClientApi.FunctinonalTests.Framework
{
    using System;
    using System.Net;

    using log4net;

    using Microsoft.Extensions.Configuration;

    using Xunit.Abstractions;

    public abstract class BaseTest
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));

        private static readonly IConfigurationRoot Configuration;

        protected const string PhoneNumber = "9996610000";
        protected const string PhoneCode = "11111";

        protected IClientApi ClientApi;
        
        static BaseTest()
        {
            Configuration = new ConfigurationBuilder()
                                 .AddJsonFile("appsettings.json")
                                 .AddJsonFile("appsettings.debug.json", true)
                                 .Build();
        }

        protected BaseTest(ITestOutputHelper output)
        {
            Log.Info(
                $"\n\n#################################################  {this} - {DateTime.Now}  ################################################################################\n\n");
            
            PrepareToTesting();
        }

        private void PrepareToTesting()
        {
            var clientTask = ClientFactory.BuildClientAsync(
                new FactorySettings
                {
                    AppHash = Configuration["AppHash"],
                    AppId = int.Parse(Configuration["AppId"]),
                    ServerAddress = Configuration["ServerAddress"],
                    ServerPublicKey = Configuration["PublicKey"],
                    ServerPort = Convert.ToInt32(Configuration["Port"]),
                    SessionTag = "session",
                    ProxyConfig = FillProxyConfig(),
                    Properties = new ApplicationProperties
                                 {
                                     AppVersion = "1.0.0",
                                     DeviceModel = "PC",
                                     LangCode = "en",
                                     LangPack = "tdesktop",
                                     SystemLangCode = "en",
                                     SystemVersion = "Win 10 Pro"
                                 }
                });

            clientTask.Wait();

            ClientApi = clientTask.Result;
        }

        private static Socks5ProxyConfig FillProxyConfig()
        {
            Socks5ProxyConfig proxyConfig = null;
            var server = Configuration["ProxyServer"];
            if (!string.IsNullOrEmpty(server))
            {
                var port = Configuration["ProxyPort"];
                var password = Configuration["ProxyPassword"];
                var username = Configuration["ProxyUsername"];
                proxyConfig = new Socks5ProxyConfig
                              {
                                  Endpoint = new IPEndPoint(IPAddress.Parse(server), int.Parse(port)),
                                  Password = string.IsNullOrEmpty(password) ? null: password,
                                  Username = string.IsNullOrEmpty(username) ? null: username,
                              };
            }

            return proxyConfig;
        }
    }
}