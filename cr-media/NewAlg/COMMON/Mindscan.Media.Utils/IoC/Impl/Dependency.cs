using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Mindscan.Media.Utils.IoC.Impl
{
	public sealed class Dependency : IDependencyResolver, IDependencyRegistrar, IDisposable
	{
		private static volatile Dependency _impl;
		static Dependency()
		{
			_impl = new Dependency();
			Resolver = _impl;
			Registrar = _impl;
		}

		private readonly IWindsorContainer _container;

		private Dependency()
		{
			_container = new WindsorContainer();
			_container.AddFacility<TypedFactoryFacility>();
			_container.Register(Component
				.For<IDependencyResolver>()
				.Instance(this)
				.LifeStyle.Singleton);
		}

		public static Dependency Instance => _impl;
		public static IDependencyResolver Resolver { get; }

		public static IDependencyRegistrar Registrar { get; }

		T IDependencyResolver.Resolve<T>(string name)
		{
			return _container.Resolve<T>(name);
		}

		public T ResolveOrDefault<T>(string name) where T : class
		{
			if (_container.Kernel.HasComponent(name))
			{
				return _container.Resolve<T>(name);
			}
			return _container.Resolve<T>();
		}

		T IDependencyResolver.Resolve<T>()
		{
			return _container.Resolve<T>();
		}

		T IDependencyResolver.Resolve<T>(Type type)
		{
			return (T)_container.Resolve(type);
		}

		IEnumerable<Type> IDependencyResolver.FindTypes(Type type)
		{
			return _container.Kernel
				.GetAssignableHandlers(type)
				.Select(h => h.ComponentModel.Implementation);
		}

		IEnumerable<Type> IDependencyResolver.FindTypes<T>(Func<Type, bool> filter)
		{
			return _container.Kernel
				.GetAssignableHandlers(typeof(T))
				.Select(h => h.ComponentModel.Implementation)
				.Where(filter)
				.ToList();
		}

		IDependencyRegistrar IDependencyRegistrar.Register<TInterface, TImpl>(Lifetime lifetime, bool asDefault)
		{
			if (!_container.Kernel.HasComponent(typeof(TInterface)))
			{
				if (asDefault)
				{
					_container.Register(Component
						.For<TInterface>()
						.ImplementedBy<TImpl>()
						.GetLifeStyle(lifetime)
						.IsDefault());
				}
				else
				{
					_container.Register(Component
						.For<TInterface>()
						.ImplementedBy<TImpl>()
						.GetLifeStyle(lifetime));
				}
				
			}
			return this;
		}

		public IDependencyRegistrar Register<TInterface1, TInterface2, TImpl>(Lifetime lifetime = Lifetime.Transient) where TInterface1 : class where TInterface2 : class where TImpl : class, TInterface1, TInterface2
		{
			if (!_container.Kernel.HasComponent(typeof(TInterface1)) && !_container.Kernel.HasComponent(typeof(TInterface2)))
			{
				_container.Register(Component
					.For<TInterface1, TInterface2>()
					.ImplementedBy<TImpl>()
					.GetLifeStyle(lifetime));
			}
			return this;
		}

		public IDependencyRegistrar Register<TInterface, TImpl>(string name, Lifetime lifetime = Lifetime.Transient, bool asDefault = false) where TInterface : class where TImpl : class, TInterface
		{
			if (!_container.Kernel.HasComponent(name))
			{
				if (asDefault)
				{
					_container.Register(Component
						.For<TInterface>()
						.ImplementedBy<TImpl>()
						.Named(name)
						.IsDefault()
						.GetLifeStyle(lifetime));
				}
				else
				{
					_container.Register(Component
						.For<TInterface>()
						.ImplementedBy<TImpl>()
						.Named(name)
						.GetLifeStyle(lifetime));
				}
			}
			return this;
		}

		IDependencyRegistrar IDependencyRegistrar.Register(Type forType, Type instance, Lifetime lifetime)
		{
			if (!_container.Kernel.HasComponent(forType))
			{
				_container.Register(Component
					.For(forType)
					.ImplementedBy(instance)
					.GetLifeStyle(lifetime));
			}
			return this;
		}

		IDependencyRegistrar IDependencyRegistrar.Register<TInterface>(TInterface instance, Lifetime lifetime)
		{
			if (!_container.Kernel.HasComponent(typeof(TInterface)))
			{
				_container.Register(Component
					.For<TInterface>()
					.Instance(instance)
					.GetLifeStyle(lifetime));
			}
			return this;
		}

		public IDependencyRegistrar Register<TInterface>(string name, TInterface instance, Lifetime lifetime = Lifetime.Transient) where TInterface : class
		{
			if (!_container.Kernel.HasComponent(name))
			{
				_container.Register(
					Component
						.For<TInterface>()
						.Named(name)
						.Instance(instance)
						.GetLifeStyle(lifetime));
			}
			return this;
		}


		IDependencyRegistrarBulk IDependencyRegistrar.FromAssembly<T>() 
		{
			return new DependencyRegistrarBulk(_container, typeof(T));
		}

		void IDependencyRegistrar.ReleaseComponent(object instance)
		{
			_container.Release(instance);
		}

		public void Dispose()
		{
			_impl = null;
			_container?.Dispose();
		}
	}
}
