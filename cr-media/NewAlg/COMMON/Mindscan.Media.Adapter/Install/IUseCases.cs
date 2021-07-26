using Mindscan.Media.Adapter.Ports.Collector;
using Mindscan.Media.Adapter.Ports.Scheduler;
using Mindscan.Media.Adapter.Ports.Scraper;

namespace Mindscan.Media.Adapter.Install
{
	public interface IUseCases
	{
		ISourcesFacade Sources { get; }
		ITriggersFacade Triggers { get; }
		IFeedsFacade Feeds { get; }
		IParsersFacade Parsers { get; }
		ICollectorFacade Collector { get; }
	}
}