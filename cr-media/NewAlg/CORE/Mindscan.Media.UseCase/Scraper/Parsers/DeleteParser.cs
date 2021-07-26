using System;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class DeleteParser
	{
		private readonly IParsersRepository _repository;

		public DeleteParser(IParsersRepository repository)
		{
			_repository = repository;
		}

		public int Delete(Parser parser)
		{
			if(parser == null)
				throw new ArgumentNullException(nameof(parser));

			var old = _repository.Find(parser.Id);
			if(old == null)
				throw new EntityNotFoundException(nameof(Parser), parser.Id);
			if(old.UpdatedAtUtc != parser.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Parser), parser.Id);

			return _repository.Delete(parser);
		}
	}
}