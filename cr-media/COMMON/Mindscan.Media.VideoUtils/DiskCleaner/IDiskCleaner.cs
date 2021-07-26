using System.Threading;

namespace Mindscan.Media.VideoUtils.DiskCleaner
{
	public interface IDiskCleaner
	{
		void Start(CancellationToken token);
	}
}