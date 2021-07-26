using System;

namespace Mindscan.Media.Messages.WebLoader
{
	public class LoadContentMessage : MessageBase
	{
		public Uri PageUrl { get; set; }
		public BrokerQueue ToQueue { get; set; }
		public string Encoding { get; set; }
		public bool UseBrowser { get; set; }
	}
}