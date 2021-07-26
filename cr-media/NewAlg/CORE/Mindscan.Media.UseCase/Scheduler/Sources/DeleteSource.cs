using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Sources
{
	public class DeleteSource
	{
		private readonly ISourcesRepository _sourcesRepository;

		public DeleteSource(ISourcesRepository sourcesRepository)
		{
			_sourcesRepository = sourcesRepository;
		}

		public int Delete(Source source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			var old = _sourcesRepository.Find(source.Id);
			if(old == null)
				throw new EntityNotFoundException(nameof(Source), source.Id);
			if(old.UpdatedAtUtc != source.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Source), source.Id);

			return _sourcesRepository.Delete(source)
				.ThrowIfZero(() => new EntityFailToDeleteException(nameof(Source), source.Id));
		}
	}
}