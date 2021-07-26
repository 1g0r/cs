using System;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class RunParser
	{
		private readonly IParserProcessor _processor;
		private readonly ValidateParserJson _validator;
		public RunParser(IParserProcessor processor, ValidateParserJson validator)
		{
			_processor = processor;
			_validator = validator;
		}
		public string Run(string json, string content, NormalizedUrl pageUrl, bool debug)
		{
			if(string.IsNullOrWhiteSpace(content))
				throw new ArgumentNullException(nameof(content));
			if(pageUrl == null)
				throw new ArgumentNullException(nameof(content));

			
			var minJson = _validator.Validate(json);
			return _processor.Execute(minJson, content, pageUrl, debug);
		}
	}
}