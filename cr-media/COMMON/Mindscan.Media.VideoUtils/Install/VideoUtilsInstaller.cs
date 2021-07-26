using Mindscan.Media.Utils.Install;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.Scheduler;
using Mindscan.Media.VideoUtils.Config;
using Mindscan.Media.VideoUtils.Config.Impl;
using Mindscan.Media.VideoUtils.DiskCleaner;
using Mindscan.Media.VideoUtils.DiskCleaner.Impl;
using Mindscan.Media.VideoUtils.FileStorage;
using Mindscan.Media.VideoUtils.FileStorage.Impl;
using Mindscan.Media.VideoUtils.VideoConverter;
using Mindscan.Media.VideoUtils.VideoConverter.Impl;

namespace Mindscan.Media.VideoUtils.Install
{
	internal sealed class VideoUtilsInstaller : IVideoUtilsInstaller
	{
		private readonly IDependencyRegistrar _registrar;

		public VideoUtilsInstaller(
			IDependencyRegistrar registrar,
			string utilsSectionName
		)
		{
			var utilsSettings = new VideoUtilsConfig(utilsSectionName);
			_registrar = registrar;
			_registrar.Register<IVideoUtilsConfig>(utilsSettings);
		}

		public IVideoUtilsInstaller UseDiscCleaner()
		{
			Dependency.Registrar
				.InstallUtils()
				.UseLogger()
				.UseScheduler();

			Dependency.Registrar
				.Register<IScheduledTask, DiscCleanerScheduledTask>("DiscCleaner", Lifetime.Singleton);

			_registrar.Register<IDiskCleaner, DiskCleaner.Impl.DiskCleaner>(Lifetime.Singleton);
			return this;
		}

		public IVideoUtilsInstaller UseFileStorageFacade()
		{
			Dependency.Registrar
				.InstallUtils()
				.UseLogger()
				.UseScheduler();

			_registrar.Register<IFileStorageFacade, FileStorageFacade>(Lifetime.Singleton);
			return this;
		}

		public IVideoUtilsInstaller UseVideoConverterFacade()
		{
			Dependency.Registrar
				.InstallUtils()
				.UseLogger();

			Dependency.Registrar
				.Register<VideoConverterFactory, VideoConverterFactory>(Lifetime.Singleton)
				.Register<IVideoHelper, VideoHelper>(Lifetime.Singleton);

			_registrar.Register<IVideoConverterFacade, VideoConverterFacade>(Lifetime.Singleton);
			return this;
		}
	}
}