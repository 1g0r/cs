namespace Mindscan.Media.Messages
{
	public sealed class BrokerQueue
	{
		public string Queue { get; set; }
		public string Exchange { get; set; }
		public string RoutingKey { get; set; }
	}
}