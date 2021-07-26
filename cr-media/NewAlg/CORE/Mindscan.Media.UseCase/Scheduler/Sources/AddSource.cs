using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Sources
{
	public class AddSource
	{
		private readonly ISourcesRepository _sourcesRepository;
		public AddSource(ISourcesRepository sourcesRepository)
		{
			_sourcesRepository = sourcesRepository;
		}

		public Source Add(Source source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			if (_sourcesRepository.Exists(source))
				throw new EntityAlreadyExistsException(nameof(Source), source.Url.ToString());

			var result = _sourcesRepository.Add(source);
			return result
				.ThrowIfNull(() => new EntityFailToAddException(nameof(Source)));
		}
	}
}