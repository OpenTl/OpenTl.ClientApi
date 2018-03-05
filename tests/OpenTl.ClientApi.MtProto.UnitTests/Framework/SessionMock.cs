namespace OpenTl.ClientApi.MtProto.UnitTests.Framework
{
    using System;

    using Moq;

    internal static class SessionMock
    {
        private static readonly Random Random = new Random();

        public static Mock<IClientSession> BuildSession(this Mock<IClientSession> mock, ulong sessionId, byte[] salt, byte[] authKeyData)
        {
            mock
                .Setup(session => session.ServerSalt)
                .Returns(salt);

            mock
                .Setup(session => session.SessionId)
                .Returns(sessionId);

            return mock;
        }

        public static Mock<IClientSession> SetupConnection(this Mock<IClientSession> mock, string serverAddress, int serverPort)
        {
            mock.Setup(session => session.ServerAddress)
                .Returns(() => serverAddress);
            
            mock.Setup(session => session.Port)
                .Returns(() => serverPort);

            return mock;
        }

        
        public static byte[] GenerateAuthKeyData()
        {
            var key = new byte[256];
            for (var i = 0; i < 256; i++)
            {
                key[i] = (byte)Random.Next(255);
            }

            return key;
        }

        public static Mock<IClientSession> Create()
        {
            return new Mock<IClientSession>();
        }
    }
}