using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Mindscan.Media.Utils.IoC.Impl
{
	internal class DependencyRegistrarBulk : IDependencyRegistrarBulk
	{
		private readonly IWindsorContainer _container;
		private readonly Type _assemblyType;
		internal DependencyRegistrarBulk(IWindsorContainer container, Type assemblyType)
		{
			_container = container;
			_assemblyType = assemblyType;
		}
		public IDependencyRegistrar BasedOn<TParent>(Lifetime lifetime = Lifetime.Transient) where TParent : class
		{
			_container.Register(
				Classes
					.FromAssemblyContaining(_assemblyType)
					.IncludeNonPublicTypes()
					.BasedOn<TParent>()
					.GetLifeStyle(lifetime)
			);

			return Dependency.Registrar;
		}

		public IDependencyRegistrar BasedOn(Type type, Lifetime lifetime = Lifetime.Transient)
		{
			_container.Register(Classes
				.FromAssemblyContaining(_assemblyType)
				.IncludeNonPublicTypes()
				.BasedOn(type)
				.GetLifeStyle(lifetime));

			return Dependency.Registrar;
		}
	}
}