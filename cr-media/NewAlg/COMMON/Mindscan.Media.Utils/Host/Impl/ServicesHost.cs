using System;
using System.Threading;
using System.Threading.Tasks;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.Logger;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace Mindscan.Media.Utils.Host.Impl
{
	internal sealed class ServicesHost : IServicesHost
	{

		private readonly IHostConfig _config;
		private readonly Topshelf.Host _host;
		private readonly CancellationTokenSource _cts;
		private readonly IMediaService _mediaService;
		private readonly ILogger _logger;
		private readonly ILoggerFactory _loggerFactory;
		private Task _serviceTask;

		public ServicesHost(IUtilsConfig config, IMediaService mediaService, ILoggerFactory factory)
		{
			_loggerFactory = factory;
			_config = config.HostConfig;
			_host = HostFactory.New(ConfigureHost);
			_mediaService = mediaService;
			_logger = factory.CreateLogger(GetType().Name);
			_cts = new CancellationTokenSource();
		}

		private void ConfigureHost(HostConfigurator host)
		{
			host.Service<ServicesHost>(s =>
			{
				s.ConstructUsing(() => this);
				s.WhenStarted(x => StartService());
				s.WhenStopped(x => StopService());
				s.WhenPaused(x => StopService());
				s.WhenContinued(x => StartService());
			});

			host.EnableShutdown();
			host.SetStartTimeout(_config.StartTimeout);
			host.SetStopTimeout(_config.StopTimeout);
			host.SetServiceName(_config.Name);
			host.SetDisplayName(_config.DisplayName);
			host.SetDescription(_config.Description);
			host.SetInstanceName(_config.InstanceName);
			host.EnableServiceRecovery(x =>
			{
				x.OnCrashOnly();
				x.RestartService(3);
				x.SetResetPeriod(1);
			});
			host.StartAutomaticallyDelayed();

			if (!string.IsNullOrEmpty(_config.User) && !string.IsNullOrEmpty(_config.Password))
			{
				host.RunAs(_config.User, _config.Password);
			}
			else
			{
				host.RunAsLocalSystem();
			}

			HostLogger.UseLogger(new LoggerConfigurator(_loggerFactory));
		}

		private void StartService()
		{
			try
			{
				_serviceTask = Task.Run(() => ExecuteService(_cts.Token), _cts.Token);
				_logger.Debug("Service is started!");
			}
			catch (Exception ex)
			{
				_logger.Fatal("Unable to start service!", ex);
				StopService();
			}
			
		}

		private void ExecuteService(CancellationToken token)
		{
			try
			{
				_mediaService.Run(token);
			}
			catch (Exception e)
			{
				_logger.Error("Service ended with error", e);
				StopService();
			}
		}

		private void StopService()
		{
			try
			{
				_mediaService.Stop(_cts.Token);
				_serviceTask?.Dispose();
			}
			catch (Exception ex)
			{
				_logger.Fatal("Unable to stop service!", ex);
			}
			finally
			{
				_cts.Cancel();
			}
		}

		public int Run()
		{
			return (int)_host.Run();
		}
	}
}