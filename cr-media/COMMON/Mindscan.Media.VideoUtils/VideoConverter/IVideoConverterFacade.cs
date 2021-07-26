using System.Threading;
using Mindscan.Media.DomainObjects.Integration;
using Mindscan.Media.Messages.Collector;

namespace Mindscan.Media.VideoUtils.VideoConverter
{
	public interface IVideoConverterFacade
	{
		void ConvertVideo(MaterialData material, string localname, CancellationToken cancellationToken);
	}
}