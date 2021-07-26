using System;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class UpdateParser
	{
		private readonly IParsersRepository _parsersRepository;
		private readonly ValidateParserJson _jsonValidator;

		public UpdateParser(IParsersRepository parsersRepository, ValidateParserJson validator)
		{
			_parsersRepository = parsersRepository;
			_jsonValidator = validator;
		}
		public Parser Update(Parser parser)
		{
			if (parser == null)
				throw new ArgumentNullException(nameof(parser));
			var old = _parsersRepository.Find(parser.Id);
			if (old == null)
				throw new EntityNotFoundException(nameof(Parser), parser.Id);
			if (old.UpdatedAtUtc != parser.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Parser), parser.Id);
			if(HasNoChanges(old, parser))
				throw new EntityHasNoChangesException(nameof(Parser), parser.Id);

			if (!string.IsNullOrWhiteSpace(parser.Json))
			{
				string minJson = _jsonValidator.Validate(parser.Json);
				if (minJson != old.Json)
				{
					parser.Json = minJson;
				}
			}

			return _parsersRepository.Update(parser)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Parser), parser.Id));
		}

		public Parser Update(Parser parser, string json)
		{
			if(parser == null)
				throw new ArgumentNullException(nameof(parser));
			if(string.IsNullOrWhiteSpace(json))
				throw new ArgumentNullException(nameof(json));

			var old = _parsersRepository.Find(parser.Id);
			if (old == null)
				throw new EntityNotFoundException(nameof(Parser), parser.Id);
			if (old.UpdatedAtUtc != parser.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Parser), parser.Id);

			string minJson = _jsonValidator.Validate(json);
			if (old.Json == minJson)
				throw new EntityHasNoChangesException(nameof(Parser), parser.Id);

			return _parsersRepository.Update(parser, minJson)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Parser), parser.Id));
		}

		private static bool HasNoChanges(Parser old, Parser @new)
		{
			if (old.Id != @new.Id)
				return true;

			return old.UseBrowser == @new.UseBrowser &&
				old.Encoding == @new.Encoding &&
				old.Host == @new.Host &&
				old.Path == @new.Path &&
				old.Tag == @new.Tag &&
				old.AdditionalInfo == @new.AdditionalInfo &&
				JsonNotChanged(old, @new);
		}

		private static bool JsonNotChanged(Parser old, Parser @new)
		{
			return string.IsNullOrWhiteSpace(@new.Json) || old.Json == @new.Json;
		}
	}
}