namespace OpenTl.ClientApi.MtProto.UnitTests.Framework
{
    using AutoFixture;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using DotNetty.Common;

    using Moq;

    using OpenTl.ClientApi.MtProto.UnitTests.Framework.IoC;
    using OpenTl.Common.IoC;
    using OpenTl.Common.Testing;
    
    public abstract class UnitTest: BaseTest
    {
        public Fixture Fixture { get; set; } = new Fixture();

        public override IWindsorContainer Container { get; } = WindsorFactory.Create();

        protected UnitTest()
        {
            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Paranoid;
            
            Container.Kernel.Resolver.AddSubResolver(new AutoMoqResolver(Container.Kernel));
            Container.Register(Component.For(typeof(Mock<>)));
        }        
    }
}