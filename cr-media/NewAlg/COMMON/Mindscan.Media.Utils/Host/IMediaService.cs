using System.Threading;

namespace Mindscan.Media.Utils.Host
{
	public interface IMediaService
	{
		void Run(CancellationToken token);
		void Stop(CancellationToken token);
	}
}