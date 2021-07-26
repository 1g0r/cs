using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.Adapter.Ports.Scraper.Impl
{
	internal sealed class ParserProcessor: IParserProcessor
	{
		public string Validate(string json)
		{
			var instance = ParserBuilder.CreateParser(json);
			return instance.ToJson(false, false);
		}

		public string Execute(string json, string content, NormalizedUrl pageUrl, bool debug)
		{
			var instance = ParserBuilder.CreateParser(json);
			if (instance == null)
				throw new ParserJsonValidationException("Unable to parse json");

			return instance.ParsePage(content, pageUrl.Value, true, debug);
		}
	}
}