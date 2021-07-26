using System;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Collector.Materials
{
	public class AddMaterial
	{
		private readonly IMaterialsRepository _materialsRepository;
		private readonly ISourcesRepository _sourcesRepository;
		private readonly IParsersRepository _parsersRepository;
		public AddMaterial(
			IMaterialsRepository materialsRepository, 
			ISourcesRepository sourcesRepository,
			IParsersRepository parsersRepository)
		{
			_materialsRepository = materialsRepository;
			_sourcesRepository = sourcesRepository;
			_parsersRepository = parsersRepository;
		}

		public Material Add(Material material)
		{
			if(material == null)
				throw new ArgumentNullException(nameof(material));

			if (_materialsRepository.Exists(material))
				throw new EntityAlreadyExistsException("Material", material.OriginalUrl.ToString());

			var source = _sourcesRepository.Find(material.SourceId);
			if(source == null)
				throw new EntityNotFoundException(nameof(Source), material.SourceId);

			if (material.ParserId.HasValue)
			{
				var parser = _parsersRepository.Find(material.ParserId.Value);
				if(parser == null)
					throw new EntityNotFoundException(nameof(Parser), material.SourceId);
			}

			material.CreatedAtUtc = material.UpdatedAtUtc = DateTime.UtcNow;

			return _materialsRepository.Add(material)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Material)));
		}
	}
}
