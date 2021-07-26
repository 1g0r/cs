using System;
using System.Collections.Generic;

namespace Mindscan.Media.Utils.IoC
{
	public interface IDependencyResolver
	{
		T Resolve<T>(string name) where T : class;
		T ResolveOrDefault<T>(string name) where T : class;
		T Resolve<T>() where T : class;
		T Resolve<T>(Type type) where T : class;
		IEnumerable<Type> FindTypes<T>(Func<Type, bool> filter) where T : class;
		IEnumerable<Type> FindTypes(Type type);
	}
}
