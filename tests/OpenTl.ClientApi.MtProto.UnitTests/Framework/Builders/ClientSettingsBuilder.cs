namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using AutoFixture;

    using Moq;

    using OpenTl.Common.Testing;
    using OpenTl.Schema;

    internal static class ClientSettingsBuilder
    {
        public static Mock<IClientSettings> BuildClientSettingsProps(this UnitTest unitTest)
        {
            var mSession = unitTest.Resolve<Mock<IClientSession>>().SetupAllProperties();

            var mAppProperties = unitTest.BuildAppProperties();

            var mSettings =  unitTest.BuildClientSettings()
                                     .AttachSession(mSession)
                                     .AttachAppProperties(mAppProperties);

            mSettings.Object.Config = unitTest.Fixture.Create<TConfig>();
            mSettings.Object.Config.DcOptions.Add(unitTest.Fixture.Create<TDcOption>());
            
            return mSettings;
        }
            
        public static Mock<IClientSettings> BuildClientSettings(this UnitTest unitTest)
        {
            var mock = unitTest.Resolve<Mock<IClientSettings>>().SetupAllProperties();
            mock
                .Setup(service => service.AppHash)
                .Returns(() => unitTest.Fixture.Create<string>());

            mock
                .Setup(service => service.AppId)
                .Returns(() => unitTest.Fixture.Create<int>());

            return mock;
        }

        public static Mock<IClientSettings> AttachSession(this Mock<IClientSettings> mock, Mock<IClientSession> mSession)
        {
            mock
                .Setup(service => service.ClientSession)
                .Returns(() => mSession.Object);

            return mock;
        }
        
        public static Mock<IClientSettings> AttachAppProperties(this Mock<IClientSettings> mock, Mock<IApplicationProperties> mAppProperties)
        {
            mock
                .Setup(service => service.ApplicationProperties)
                .Returns(() => mAppProperties.Object);

            return mock;
        }
    }
}