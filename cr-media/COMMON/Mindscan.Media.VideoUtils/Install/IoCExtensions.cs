using Mindscan.Media.Utils.IoC;

namespace Mindscan.Media.VideoUtils.Install
{
	public static class IoCExtensions
	{
		public static IVideoUtilsInstaller InstallVideoUtils(this IDependencyRegistrar registrar, string utilsSectionName = "utils")
		{
			return new VideoUtilsInstaller(registrar, utilsSectionName);
		}
	}
}
