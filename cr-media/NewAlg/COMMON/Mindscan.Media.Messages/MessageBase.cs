using System;

namespace Mindscan.Media.Messages
{
	public abstract class MessageBase
	{
		protected MessageBase()
		{
			
		}

		protected MessageBase(MessageBase messageToCopy)
		{
			FeedId = messageToCopy.FeedId;
			FeedUrl = messageToCopy.FeedUrl;
			Context = messageToCopy.Context;
		}
		public long FeedId { get; set; }
		public Uri FeedUrl { get; set; }
		public string Context { get; set; }
	}
}