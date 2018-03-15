namespace OpenTl.ClientApi.MtProto.FunctionalTests.Framework
{
    using System;
    using System.Threading.Tasks;

    using log4net;

    using Microsoft.Extensions.Configuration;

    using Xunit.Abstractions;

    public abstract class BaseTest
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));

        private readonly LogOutputTester _logOutputTester;

        private static readonly IConfigurationRoot Configuration;
        
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
            
        }

        protected async Task<IClientApi> GenerateClientApi(int index)
        {
            return await ClientFactory.BuildClient(
                new FactorySettings
                {
                    AppHash = Configuration["AppHash"],
                    AppId = int.Parse(Configuration["AppId"]),
                    ServerAddress = Configuration["ServerAddress"],
                    ServerPublicKey = Configuration["PublicKey"],
                    ServerPort = Convert.ToInt32(Configuration["Port"]),
                    SessionTag = "session_" + index,
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
        }
    }
}