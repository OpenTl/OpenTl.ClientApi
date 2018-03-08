namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using System.Threading.Tasks;

    using Moq;

    public static class SessionWriterBuilder
    {
        public static Mock<ISessionWriter> BuildSuccessSave(this Mock<ISessionWriter> mock)
        {
            mock
                .Setup(service => service.Save(It.IsAny<IClientSession>()))
                .Returns(() => Task.CompletedTask);

            return mock;
        }
    }
}