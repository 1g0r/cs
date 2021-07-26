using System;

namespace Mindscan.Media.UseCase.OLD
{
	public interface IMaterialsRepository
	{
		DateTimeOffset? GetPublicationDate(Uri sourceUrl);
		DateTimeOffset? UpdatePublicationDate(Uri sourceUrl, DateTimeOffset date, Uri pageUrl);
	}
}
