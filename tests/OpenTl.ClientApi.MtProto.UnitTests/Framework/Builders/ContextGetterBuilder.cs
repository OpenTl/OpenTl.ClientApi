namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders
{
    using Castle.Windsor;

    using DotNetty.Transport.Channels;

    using Moq;

    using OpenTl.ClientApi.MtProto.Interfaces;

    internal static class ContextGetterBuilder
    {
        public static void Build(IWindsorContainer container)
        {
            var mContextGetter = container.Resolve<Mock<IContextGetter>>();
            mContextGetter.Setup(getter => getter.Context)
                          .Returns(() => container.Resolve<Mock<IChannelHandlerContext>>().Object);
        }
    }
}