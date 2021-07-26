using System.Threading;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.UseCase.Ports
{
	public interface ITriggerStarter
	{
		void Fire(Trigger trigger, CancellationToken token);
	}
}