using System;

namespace Mindscan.Media.Utils.IoC
{
	public interface IDependencyRegistrarBulk
	{
		IDependencyRegistrar BasedOn<TParent>(Lifetime lifetime = Lifetime.Transient) where TParent : class;
		IDependencyRegistrar BasedOn(Type type, Lifetime lifetime = Lifetime.Transient);
	}
}