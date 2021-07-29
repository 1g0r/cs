using Mindscan.Media.Scheduler.Host.Service;
using Mindscan.Media.Utils.Host;
using Mindscan.Media.Utils.Install;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;

namespace Mindscan.Media.Scheduler.Host
{
	class Program
	{
		static int Main(string[] args)
		{
			Dependency.Registrar.InstallUtils("utils")
				.UseLogger()
				.UseHost();

			Dependency.Registrar
				.Register<IMediaService, SchedulerService>(Lifetime.Singleton);


			var service = Dependency.Resolver.Resolve<IServicesHost>();
			return service.Run();
		}
	}
}
