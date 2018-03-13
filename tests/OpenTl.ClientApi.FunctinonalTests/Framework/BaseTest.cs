namespace OpenTl.ClientApi.MtProto.FunctionalTests.Framework
{
    using System;

    using log4net;

    using Microsoft.Extensions.Configuration;

    using Xunit.Abstractions;

    public abstract class BaseTest
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));

        private readonly LogOutputTester _logOutputTester;

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
            _logOutputTester = new LogOutputTester(output);

            Log.Info(
                $"\n\n#################################################  {this} - {DateTime.Now}  ################################################################################\n\n");
            
            PrepareToTesting();
            
        }

        private void PrepareToTesting()
        {
            var clientTask = ClientFactory.BuildClient(
                new FactorySettings
                {
                    AppHash = Configuration["AppHash"],
                    AppId = int.Parse(Configuration["AppId"]),
                    ServerAddress = Configuration["ServerAddress"],
                    ServerPublicKey = Configuration["PublicKey"],
                    ServerPort = Convert.ToInt32(Configuration["Port"]),
                    SessionTag = "session",
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
    }
}