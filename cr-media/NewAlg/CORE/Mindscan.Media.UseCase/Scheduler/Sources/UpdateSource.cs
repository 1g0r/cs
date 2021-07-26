using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Sources
{
	public class UpdateSource
	{
		private readonly ISourcesRepository _sourcesRepository;
		public UpdateSource(ISourcesRepository sourcesRepository)
		{
			_sourcesRepository = sourcesRepository;
		}

		public Source Update(Source source)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));
			var oldSource = _sourcesRepository.Find(source.Id);
			if (oldSource == null)
				throw new EntityNotFoundException(nameof(Source), source.Id);
			if(oldSource.UpdatedAtUtc != source.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Source), source.Id);

			if (HasNoChanges(oldSource, source))
				throw new EntityHasNoChangesException(nameof(Source), source.Id);

			return _sourcesRepository.Update(source)
				.ThrowIfNull(() => new EntityFailToUpdateException(nameof(Source), source.Id));
		}

		private bool HasNoChanges(Source oldSource, Source newSource)
		{
			if (oldSource.Id != newSource.Id)
				return true;

			return oldSource.Url.Tail == newSource.Url.Tail &&
				oldSource.Type == newSource.Type &&
				oldSource.Name == newSource.Name &&
				oldSource.AdditionalInfo == newSource.AdditionalInfo;
		}
	}
}