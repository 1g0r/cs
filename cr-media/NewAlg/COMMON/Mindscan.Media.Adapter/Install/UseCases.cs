using System;
using System.Diagnostics;
using Mindscan.Media.Adapter.Ports.Collector;
using Mindscan.Media.Adapter.Ports.Collector.Impl;
using Mindscan.Media.Adapter.Ports.Scheduler;
using Mindscan.Media.Adapter.Ports.Scheduler.Impl;
using Mindscan.Media.Adapter.Ports.Scraper;
using Mindscan.Media.Adapter.Ports.Scraper.Impl;
using Mindscan.Media.UseCase.Ports;


namespace Mindscan.Media.Adapter.Install
{
	[DebuggerStepThrough]
	internal class UseCases : IUseCases
	{
		private readonly Lazy<ISourcesFacade> _sources;
		private readonly Lazy<ITriggersFacade> _triggers;
		private readonly Lazy<IFeedsFacade> _feeds;
		private readonly Lazy<IParsersFacade> _parsers;
		private readonly Lazy<ICollectorFacade> _materials;
		public UseCases(
			ISourcesRepository sources,
			IFeedsRepository feeds,
			ITriggersRepository triggers, 
			ITriggerStarter starter,
			IParsersRepository parsers,
			IParserProcessor validator,
			IMaterialsRepository materials,
			ICache<bool> boolCache)
		{
			_sources = new Lazy<ISourcesFacade>(() => new SourcesFacade(sources));
			_triggers = new Lazy<ITriggersFacade>(() => new TriggersFacade(triggers, feeds, sources, starter));
			_feeds = new Lazy<IFeedsFacade>(() => new FeedsFacade(feeds, sources) );
			_parsers = new Lazy<IParsersFacade>(() => new ParsersFacade(sources, parsers, validator));
			_materials = new Lazy<ICollectorFacade>(() => new CollectorFacade(materials, sources, parsers, boolCache));
		}

		public ISourcesFacade Sources => _sources.Value;
		public ITriggersFacade Triggers => _triggers.Value;
		public IFeedsFacade Feeds => _feeds.Value;
		public IParsersFacade Parsers => _parsers.Value;
		public ICollectorFacade Collector => _materials.Value;
	}
}