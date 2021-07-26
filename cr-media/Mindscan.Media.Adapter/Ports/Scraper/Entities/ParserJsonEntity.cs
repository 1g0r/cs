using System;

namespace Mindscan.Media.Adapter.Ports.Scraper.Entities
{
	internal class ParserJsonEntity
	{
		public long Id { get; set; }
		public DateTime CreatedAtUtc { get; set; }
		public bool Enabled { get; set; }
		public string Value { get; set; }
	}
}