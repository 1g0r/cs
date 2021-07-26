using System;

namespace Mindscan.Media.Utils.IoC
{
	public interface IDependencyRegistrar
	{
		IDependencyRegistrar Register<TInterface, TImpl>(Lifetime lifetime = Lifetime.Transient, bool asDefault = false)
			where TInterface : class
			where TImpl : class, TInterface;

		IDependencyRegistrar Register<TInterface1, TInterface2, TImpl>(Lifetime lifetime = Lifetime.Transient)
			where TInterface1 : class
			where TInterface2 : class
			where TImpl : class, TInterface1, TInterface2;

		IDependencyRegistrar Register<TInterface, TImpl>(string name, Lifetime lifetime = Lifetime.Transient, bool asDefault = false)
			where TInterface : class
			where TImpl : class, TInterface;

		IDependencyRegistrar Register(Type forType, Type instance, Lifetime lifetime = Lifetime.Transient);

		IDependencyRegistrar Register<TInterface>(TInterface instance, Lifetime lifetime = Lifetime.Transient)
			where TInterface : class;

		IDependencyRegistrar Register<TInterface>(string name, TInterface instance, Lifetime lifetime = Lifetime.Transient)
			where TInterface : class;

		IDependencyRegistrarBulk FromAssembly<T>() where T : class;
		
		void ReleaseComponent(object instance);
	}
}