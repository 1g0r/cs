using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.OLD;
using Mindscan.Media.Adapter.OLD.Impl;
using Mindscan.Media.Adapter.Ports.Collector.Impl;
using Mindscan.Media.Adapter.Ports.Scheduler.Impl;
using Mindscan.Media.Adapter.Ports.Scraper.Impl;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Install;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.Logger;

using Old = Mindscan.Media.UseCase.OLD;

namespace Mindscan.Media.Adapter.Install
{
	public static class IoCExtensions
	{
		public static IDependencyRegistrar InstallUseCases(this IDependencyRegistrar registrar, string configName = "repository")
		{
			registrar.InstallUtils("utils")
				.UseLogger()
				.UseBroker();

			var config = new RepositoryConfig(configName);
			registrar.Register<IRepositoryConfig>(config, Lifetime.Singleton)
				.Register<ISourcesRepository, DbSourcesRepository>(Lifetime.Singleton)
				.Register<IFeedsRepository, DbFeedsRepository>(Lifetime.Singleton)
				.Register<ITriggersRepository, DbTriggersRepository>(Lifetime.Singleton)
				.Register<IParserProcessor, ParserProcessor>(Lifetime.Singleton)
				.Register<IParsersRepository, DbParserRepository>(Lifetime.Singleton)
				.Register<IUseCases, UseCases>(Lifetime.Singleton)
				.Register<IMaterialsRepository, MaterialsRepository>(Lifetime.Singleton)
				.Register<ITriggerStarter, TriggerStarter>(Lifetime.Singleton)
				.Register<ICache<bool>, Cache<bool>>(Lifetime.Singleton);

			return registrar;
		}

		public static IDependencyRegistrar InstallOldUseCases(this IDependencyRegistrar registrar, string configName = "repository")
		{
			registrar.InstallUtils()
				.UseLogger()
				.UseBroker();

			var factory = Dependency.Resolver.Resolve<ILoggerFactory>();
			var config = new RepositoryConfig(configName);
			registrar
				.Register<Old.IMaterialsRepository>(new DbMaterialsOldRepository(factory, config), Lifetime.Singleton)
				.Register<IPublicationDateFacade, PublicationDateFacade>(Lifetime.Singleton);
			return registrar;
		}
	}
}
