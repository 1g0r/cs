using System;
using System.Collections.Generic;

namespace Mindscan.Media.Messages
{
	public static class MessageHeaderExtensions
	{
		public static Dictionary<string, string> GetMediaHeaders(this MessageBase message, Uri pageUrl)
		{
			return new Dictionary<string, string>
			{
				{"MEDIA-FeedId", message?.FeedId.ToString() ?? "0"},
				{"MEDIA-FeedUrl", message?.FeedUrl?.ToString() ?? "empty"},
				{"MEDIA-PageUrl", pageUrl?.ToString() ?? "empty"}
			};
		}
	}
}