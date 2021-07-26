using Castle.MicroKernel.Registration;

namespace Mindscan.Media.Utils.IoC.Impl
{
	internal static class LifeTimeMapper
	{
		public static ComponentRegistration<T> GetLifeStyle<T>(this ComponentRegistration<T> c, Lifetime lifetime)
			where T : class
		{
			switch (lifetime)
			{
				case Lifetime.Singleton:
					return c.LifeStyle.Singleton;
				case Lifetime.PerWebRequest:
					return c.LifeStyle.PerWebRequest;
				case Lifetime.PerThread:
					return c.LifeStyle.PerThread;
				default:
					return c.LifestyleTransient();

			}
		}

		public static BasedOnDescriptor GetLifeStyle(this BasedOnDescriptor d, Lifetime lifetime)
		{
			switch (lifetime)
			{
				case Lifetime.Singleton:
					return d.LifestyleSingleton();
				case Lifetime.PerWebRequest:
					return d.LifestylePerWebRequest();
				case Lifetime.PerThread:
					return d.LifestylePerThread();
				default:
					return d.LifestyleTransient();
			}
		}
	}
}