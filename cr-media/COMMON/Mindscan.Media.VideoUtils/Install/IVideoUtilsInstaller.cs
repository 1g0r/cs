namespace Mindscan.Media.VideoUtils.Install
{
	public interface IVideoUtilsInstaller
	{
		IVideoUtilsInstaller UseDiscCleaner();
		IVideoUtilsInstaller UseVideoConverterFacade();
		IVideoUtilsInstaller UseFileStorageFacade();
	}
}