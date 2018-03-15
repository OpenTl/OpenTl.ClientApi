namespace OpenTl.ClientApi.SampeApp
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using CommandLine;

    using DotNetty.Common.Internal.Logging;

    using log4net;
    using log4net.Config;

    using Microsoft.Extensions.Configuration;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.SampeApp.Commands;
    using OpenTl.ClientApi.SampeApp.Helpers;
    using OpenTl.Common.Testing.Logger;

    internal static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            var configuration = Initialize();

            var client = new Client();
            client.Init(
                new FactorySettings
                {
                    AppHash = configuration["AppHash"],
                    AppId = int.Parse(configuration["AppId"]),
                    ServerAddress = configuration["ServerAddress"],
                    ServerPublicKey = configuration["PublicKey"],
                    ServerPort = Convert.ToInt32(configuration["Port"]),
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
                }).Wait();

            while (true)
            {
                var input = ReadLineHelper.Read("(prompt)>");

                Parser.Default.ParseArguments<SignInOptions, LogOutOptions>(input.Split(' '))
                      .MapResult<SignInOptions, LogOutOptions, Task>(
                          opt => client.SignIn(opt.Phone),
                          opt => client.LogOut(),
                          errors => Task.FromResult(1))
                      .Wait();
            }
        }

        private static IConfigurationRoot Initialize()
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile("appsettings.debug.json", true)
                                .Build();
            ConfigureLogger();
            return configuration;
        }

        private static void ConfigureLogger()
        {
            var repo = LogManager.GetRepository(typeof(INettyBootstrapper).GetTypeInfo().Assembly);
            XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));

            InternalLoggerFactory.DefaultFactory.AddProvider(new Log4NetProvider(repo));
            
            Log.Info(
                $"\n\n#################################################  {DateTime.Now}  ################################################################################\n\n");
        }
    }
}