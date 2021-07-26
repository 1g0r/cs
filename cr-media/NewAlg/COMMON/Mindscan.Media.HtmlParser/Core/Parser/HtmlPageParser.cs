using System;
using System.Collections.Generic;
using System.IO;
using Mindscan.Media.HtmlParser.Builder.Converters;
using Mindscan.Media.HtmlParser.Core.Schema;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Parser
{
	[CustomJsonObject(SupportedNames.Parsers.Namespace, SupportedNames.Parsers.Base)]
	internal sealed class HtmlPageParser : CustomJsonObject, IPageParser
	{
		[JsonProperty(nameof(Encoding), Order = 1)]
		private string _encoding;

		[JsonIgnore]
		public string Encoding
		{
			get
			{
				if (string.IsNullOrEmpty(_encoding))
				{
					return System.Text.Encoding.UTF8.HeaderName;
				}
				return _encoding;
			}
			set { _encoding = value; }
		}

		[JsonProperty(Order = 2)]
		public ComplexValue Schema { get; set; }

		public string ParsePage(Stream content, Uri pageUrl, bool indented, bool debug)
		{
			using (var reader = new StreamReader(content, System.Text.Encoding.GetEncoding(Encoding)))
			{
				return Parse(reader.ReadToEnd(), pageUrl, indented, debug);
			}
		}

		public string ParsePage(string content, Uri pageUri, bool indented, bool debug)
		{
			return Parse(content, pageUri, indented, debug);
		}

		public string ToJson(bool buildCode, bool indented)
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = indented ? Formatting.Indented : Formatting.None,
				Converters = new List<JsonConverter>
				{
					new CustomJsonWriter(buildCode)
				}
			};
			return JsonConvert.SerializeObject(this, settings);
		}

		private string Parse(string content, Uri pageUrl, bool indented, bool debug)
		{
			var parserContext = new ParserContext(pageUrl, debug);
			var result = Schema.Parse(new ExpressionContext(parserContext), content);
			if (result != null)
			{
				result["Url"] = pageUrl;
				return result.ToString(indented ? Formatting.Indented : Formatting.None);
			}
			return string.Empty;
		}
	}
}
