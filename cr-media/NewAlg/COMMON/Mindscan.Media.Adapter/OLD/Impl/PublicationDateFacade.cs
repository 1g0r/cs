using System;
using Mindscan.Media.UseCase.OLD;

namespace Mindscan.Media.Adapter.OLD.Impl
{
	internal sealed class PublicationDateFacade : IPublicationDateFacade
	{
		private readonly Lazy<PublicationDate> _date;
		public PublicationDateFacade(IMaterialsRepository repository)
		{
			_date = new Lazy<PublicationDate>(() => new PublicationDate(repository));
		}
		public DateTimeOffset? Get(Uri sourceUrl)
		{
			return _date.Value.Get(sourceUrl);
		}

		public DateTimeOffset? Update(Uri sourceUrl, DateTimeOffset date, Uri pageUrl)
		{
			return _date.Value.Update(sourceUrl, date, pageUrl);
		}
	}
}