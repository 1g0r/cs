using System;

namespace Mindscan.Media.Messages.WebLoader
{
	public class ContentLoadedMessage : MessageBase
	{
		public ContentLoadedMessage() { }

		public ContentLoadedMessage(LoadContentMessage message)
			: base(message)
		{
			PageUrl = message.PageUrl;
		}
		public Uri PageUrl { get; set; }
		public string Content { get; set; }
		public Uri RedirectedPageUrl { get; set; }
	}
}