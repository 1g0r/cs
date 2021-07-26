namespace Mindscan.Media.Messages.Smi
{
	public static class SmiQueues
	{
		public static BrokerQueue Scheduler = new BrokerQueue
		{
			Exchange = CommonExchanges.Scheduler,
			RoutingKey = "Smi",
			Queue = CommonExchanges.Scheduler
		};

		public static BrokerQueue BoardParser = new BrokerQueue
		{
			Exchange = "BoardParser",
			Queue = "BoardParser"
		};

		public static BrokerQueue PageParser = new BrokerQueue
		{
			Exchange = "PageParser",
			Queue = "PageParser"
		};

		public static BrokerQueue PageParserCleaner = new BrokerQueue
		{
			Exchange = "PageParser.Cleaner",
			Queue = "PageParser.Cleaner"
		};

		public static BrokerQueue Collector = new BrokerQueue
		{
			Exchange = CommonExchanges.Collector,
			Queue = CommonExchanges.Collector
		};

		public static BrokerQueue WebLoader = new BrokerQueue
		{
			Exchange = CommonExchanges.WebLoader,
			Queue = CommonExchanges.WebLoader
		};

		public static BrokerQueue FeedParserShort = new BrokerQueue
		{
			Exchange = "FeedParserShort",
			Queue = "FeedParserShort"
		};
	}
}
