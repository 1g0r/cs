using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.VideoUtils.DiskCleaner
{
	public interface IDiskCleanerConfig : IConfig
	{
		string RootDirectory { get; }
		int HoursOfDeleteDelay { get; }
		int HoursOfExceptionDelay { get; }
		int GBytesToKeepOnRoot { get; }
	}
}