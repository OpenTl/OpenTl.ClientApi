namespace OpenTl.ClientApi.MtProto.UnitTests.Framework.IoC
{
    using System.Reflection;

    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Context;

    using Moq;

    public class AutoMoqResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;
 
        public AutoMoqResolver(IKernel kernel)
        {
            this._kernel = kernel;
        }
 
        public bool CanResolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            return dependency.TargetType.GetTypeInfo().IsInterface;
        }
 
        public object Resolve(
            CreationContext context,
            ISubDependencyResolver contextHandlerResolver,
            ComponentModel model,
            DependencyModel dependency)
        {
            var mockType = typeof(Mock<>).MakeGenericType(dependency.TargetType);
            return ((Mock)this._kernel.Resolve(mockType)).Object;
        }
    }}