using System;

namespace Mindscan.Media.Adapter.OLD
{
	public interface IPublicationDateFacade
	{
		DateTimeOffset? Get(Uri sourceUrl);
		DateTimeOffset? Update(Uri sourceUrl, DateTimeOffset date, Uri pageUrl);
	}
}