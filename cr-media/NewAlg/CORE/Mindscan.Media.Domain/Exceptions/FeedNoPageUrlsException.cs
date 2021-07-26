using System;
using Mindscan.Media.Domain.Entities;

namespace Mindscan.Media.Domain.Exceptions
{
	public class FeedNoPageUrlsException : Exception
	{
		public FeedNoPageUrlsException(long feedId, NormalizedUrl feedUrl)
			:base($"No page urls was found from the feed with id={feedId} and url={feedUrl}")
		{
			
		}
	}
}
