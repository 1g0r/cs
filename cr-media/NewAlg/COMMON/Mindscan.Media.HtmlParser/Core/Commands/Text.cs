using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.Helpers;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Text)]
	internal class Text : CommandBase<string, IElement, XElement, JToken, IEnumerable<IElement>>
	{
		[JsonProperty(Order = 1)]
		public List<string> SkipPatterns { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			SkipPatterns.AddNotEmptyItems(json);
		}

		protected internal override string BuildParametersJson()
		{
			return SkipPatterns.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, string value)
		{
			return value.ClearString();
		}

		protected override object Execute(ParserContext context, IElement element)
		{
			return element
				.GetText(SkipPatterns.ToSkipPatterns())
				.ClearString();
		}

		protected override object Execute(ParserContext context, XElement element)
		{
			return element
				.GetText(SkipPatterns.ToSkipPatterns())
				.ClearString();
		}

		protected override object Execute(ParserContext context, JToken token)
		{
			if (token.Type == JTokenType.Date)
			{
				return token.Value<DateTime>().ToString("o", CultureInfo.InvariantCulture);
			}
			return token.Value<string>().ClearString();
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			var result = new List<string>();
			foreach (var element in elements)
			{
				var text = Execute(context, element) as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					result.Add(text);
				}
			}
			return result.Distinct().ToResult();
		}
	}
}