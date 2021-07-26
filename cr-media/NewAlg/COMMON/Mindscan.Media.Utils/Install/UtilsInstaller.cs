using System;
using System.IO;
using System.Linq;
using System.Threading;
using log4net.Appender;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Broker.Impl;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.Config.Impl;
using Mindscan.Media.Utils.DbConnector;
using Mindscan.Media.Utils.DbConnector.Impl;
using Mindscan.Media.Utils.Host;
using Mindscan.Media.Utils.Host.Impl;
using Mindscan.Media.Utils.HttpExecutor;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Logger.Impl;
using Mindscan.Media.Utils.ObjectPool;
using Mindscan.Media.Utils.ObjectPool.Impl;
using Mindscan.Media.Utils.Proxy;
using Mindscan.Media.Utils.Proxy.Impl;
using Mindscan.Media.Utils.Retry;
using Mindscan.Media.Utils.Retry.Impl;
using Mindscan.Media.Utils.Scheduler;
using Mindscan.Media.Utils.Scheduler.Impl;

namespace Mindscan.Media.Utils.Install
{
	internal sealed class UtilsInstaller: IUtilsInstaller
	{
		private readonly IDependencyRegistrar _registrar;
		private int _useLogger = 0;
		public UtilsInstaller(IDependencyRegistrar registrar, string utilsSectionName)
		{
			var utilsSettings = new UtilsConfig(utilsSectionName);
			_registrar = registrar;
			_registrar.Register<IUtilsConfig>(utilsSettings);
			registrar.Register<IResourcePoolConfig>(utilsSettings.ProxyRepositoryConfig);
		}
		public IUtilsInstaller UseRoundRobinProxySwitcher()
		{
			UseLogger();
			_registrar
				.Register<IResourceLoader<ProxyWrapper>, IProxyFinder, ProxyLoader>(Lifetime.Singleton)
				.Register<IResourcePool<ProxyWrapper>, ResourcePool<ProxyWrapper>>(Lifetime.Singleton)
				.Register<IProxySwitcher, ProxySwitcher>(Lifetime.Singleton);
			return this;
		}

		public IUtilsInstaller UseHttpExecutor()
		{
			_registrar.Register<IHttpExecutor, HttpExecutor.Impl.HttpExecutor>(Lifetime.Singleton);
			return this;
		}

		public IUtilsInstaller UseLogger()
		{
			if (Interlocked.CompareExchange(ref _useLogger, 1, 0) == 0)
			{
				var settings = Dependency.Resolver.Resolve<IUtilsConfig>().LoggerConfig;
				var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.ConfigPath.TrimStart('\\'));
				log4net.Config.XmlConfigurator.Configure(new FileInfo(logPath));
				var repository = log4net.LogManager.GetRepository();
				foreach (var fileAppender in repository.GetAppenders().OfType<FileAppender>())
				{
					if (fileAppender.Name.Equals("ErrorFileAppender") && !string.IsNullOrEmpty(settings.ErrorFilePath))
					{
						fileAppender.File = settings.ErrorFilePath;
					}
					if (fileAppender.Name.Equals("DebugFileAppender") && !string.IsNullOrEmpty(settings.DebugFilePath))
					{
						fileAppender.File = settings.DebugFilePath;
					}
					fileAppender.ActivateOptions();
				}

				_registrar.Register<ILoggerFactory, LoggerFactory>(Lifetime.Singleton);
			}
			return this;
		}

		public IUtilsInstaller UseHost()
		{
			UseLogger();
			_registrar
				.Register<IServicesHost, ServicesHost>();

			return this;
		}

		public IUtilsInstaller UseDbConnector()
		{
			_registrar
				.Register<IDataBaseConnector, DataBaseConnector>(Lifetime.Singleton);
			return this;
		}

		public IUtilsInstaller UseBroker()
		{
			UseLogger();
			UseRetry();
			_registrar.Register<IMessageBrokerBuilder, MessageBrokerBuilder>();
			return this;
		}

		public IUtilsInstaller UseRetry()
		{
			_registrar.Register<IRetryBuilder, RetryBuilder>(Lifetime.Singleton);
			return this;
		}

		public IUtilsInstaller UseScheduler()
		{
			UseLogger();
			_registrar.Register<ITaskSchedulerFactory, TaskSchedulerFactory>(Lifetime.Singleton);
			return this;
		}

		public IUtilsInstaller UseResourcePool<T>() where T : PoolItemBase
		{
			UseLogger();
			_registrar.Register<IResourcePool<T>, ResourcePool<T>>();
			return this;
		}
	}
}