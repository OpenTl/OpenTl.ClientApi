namespace OpenTl.ClientApi.MtProto.UnitTests.Framework
{
    using System;

    using Moq;

    internal static class ClientSettingsMock
    {
        public static Mock<IClientSettings> BuildClientSettings(this Mock<IClientSettings> mock, Func<int> appIdFunc = null, Func<string> appHashFunc = null)
        {
            if (appHashFunc == null)
            {
                appHashFunc = () => string.Empty;
            }

            if (appIdFunc == null)
            {
                appIdFunc = () => 0;
            }

            mock
                .Setup(service => service.AppHash)
                .Returns(appHashFunc);

            mock
                .Setup(service => service.AppId)
                .Returns(appIdFunc);

            return mock;
        }

        public static Mock<IClientSettings> AttachSession(this Mock<IClientSettings> mock, Func<IClientSession> sessionFunc)
        {
            mock
                .Setup(service => service.ClientSession)
                .Returns(sessionFunc);

            return mock;
        }

        public static Mock<IClientSettings> Create()
        {
            return new Mock<IClientSettings>();
        }
    }
}