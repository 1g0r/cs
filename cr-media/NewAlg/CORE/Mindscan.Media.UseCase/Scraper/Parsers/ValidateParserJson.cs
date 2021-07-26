using System;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class ValidateParserJson
	{
		private readonly IParserProcessor _parserProcessor;
		public ValidateParserJson(IParserProcessor processor)
		{
			_parserProcessor = processor;
		}

		public string Validate(string json)
		{
			if(string.IsNullOrWhiteSpace(json))
				throw new ArgumentNullException(nameof(json));
			try
			{
				return _parserProcessor.Validate(json);
			}
			catch (Exception ex)
			{
				throw new ParserJsonValidationException(ex.Message, ex);
			}
		}
	}
}