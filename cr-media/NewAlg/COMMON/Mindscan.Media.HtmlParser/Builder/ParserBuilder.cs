using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Builder.Converters;
using Mindscan.Media.HtmlParser.Core.Parser;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Builder
{
	public static class ParserBuilder
	{
		public static IPageParser CreateParser(out IPipelineSupportedCommands pipelineSupportedCommands)
		{
			pipelineSupportedCommands = new PipelineSupportedCommands();
			return new HtmlPageParser();
		}

		public static IPageParser CreateParser(string json)
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = Formatting.Indented,
				Converters = new List<JsonConverter>
				{
					new CustomJsonReader()
				}
			};
			return JsonConvert.DeserializeObject<IPageParser>(json, settings);
		}
	}
}