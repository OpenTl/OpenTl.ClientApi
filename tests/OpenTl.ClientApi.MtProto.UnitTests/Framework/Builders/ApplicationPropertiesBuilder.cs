namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using AutoFixture;

    using Moq;

    using OpenTl.Common.Testing;

    internal static class ApplicationPropertiesBuilder
    {
        private static readonly Fixture Fixture = new Fixture();
        
        public static Mock<IApplicationProperties> BuildAppProperties(this UnitTest unitTest)
        {
            var mock = unitTest.Resolve<Mock<IApplicationProperties>>().SetupAllProperties();
            
            mock.Setup(properties => properties.AppVersion)
                .Returns(() => Fixture.Create<string>());
            
            mock.Setup(properties => properties.DeviceModel)
                .Returns(() => Fixture.Create<string>());
            
            mock.Setup(properties => properties.LangCode)
                .Returns(() => Fixture.Create<string>());
            
            mock.Setup(properties => properties.LangPack)
                .Returns(() => Fixture.Create<string>());
            
            mock.Setup(properties => properties.SystemLangCode)
                .Returns(() => Fixture.Create<string>());
            
            mock.Setup(properties => properties.SystemVersion)
                .Returns(() => Fixture.Create<string>());

            return mock;
        }
    }
}