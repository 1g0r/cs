using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class FindParserJson
	{
		private readonly IParsersRepository _parsersRepository;
		public FindParserJson(IParsersRepository parsersRepository)
		{
			_parsersRepository = parsersRepository;
		}

		public string FindJson(long parserId)
		{
			return _parsersRepository.FindJson(parserId)
				.ThrowIfNull(()=> new EntityNotFoundException("ParserJson", parserId));
		}
	}
}