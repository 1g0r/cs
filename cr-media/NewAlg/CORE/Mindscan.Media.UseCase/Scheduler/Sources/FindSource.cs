using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Sources
{
	public class FindSource
	{
		private readonly ISourcesRepository _sourcesRepository;

		public FindSource(ISourcesRepository sourcesRepository)
		{
			_sourcesRepository = sourcesRepository;
		}

		public Source Find(long id)
		{
			return _sourcesRepository.Find(id)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Source), id));
		}

		public IEnumerable<Source> Find(SourceFilter filter)
		{
			if(filter == null)
				throw new ArgumentNullException(nameof(filter));

			filter.SetDefaults();

			return _sourcesRepository.Find(filter)
                .EnsureNotNull();
		}
	}
}