using System;

namespace Mindscan.Media.UseCase.OLD
{
	public sealed class PublicationDate
	{
		private readonly IMaterialsRepository _repository;
		public PublicationDate(IMaterialsRepository repository)
		{
			_repository = repository;
		}

		public DateTimeOffset? Get(Uri sourceUrl)
		{
			if (sourceUrl == null)
				throw new ArgumentNullException(nameof(sourceUrl));
			return _repository.GetPublicationDate(sourceUrl);
		}

		public DateTimeOffset? Update(Uri sourceUrl, DateTimeOffset date, Uri pageUrl)
		{
			if (sourceUrl == null)
				throw new ArgumentNullException(nameof(sourceUrl));
			if (pageUrl == null)
				throw new ArgumentNullException(nameof(pageUrl));
			return _repository.UpdatePublicationDate(sourceUrl, date, pageUrl);
		}
	}
}