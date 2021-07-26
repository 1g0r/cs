using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class AddParser
	{
		private readonly ISourcesRepository _sourcesRepository;
		private readonly IParsersRepository _parserRepository;
		public AddParser(ISourcesRepository sourcesRepository, IParsersRepository parserRepository)
		{
			_sourcesRepository = sourcesRepository;
			_parserRepository = parserRepository;
		}
		public Parser Add(Parser parser)
		{
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));

			var source = _sourcesRepository.Find(parser.SourceId);
			if(source == null)
				throw new EntityNotFoundException(nameof(Source), parser.SourceId);

			return _parserRepository.Add(parser)
				.ThrowIfNull(() => new EntityFailToAddException(nameof(Parser)));
		}
	}
}
