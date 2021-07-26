using Mindscan.Media.Utils.IoC;

namespace Mindscan.Media.Utils.Install
{
	public static class IoCExtensions
	{
		public static IUtilsInstaller InstallUtils(this IDependencyRegistrar registrar, string utilsSectionName = "utils")
		{
			return new UtilsInstaller(registrar, utilsSectionName);
		}
	}
}
