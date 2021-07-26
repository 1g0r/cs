using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.TakeLast)]
	internal class TakeLast : CommandBase<IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>>
	{
		[JsonProperty(Order = 1)]
		public List<string> Patterns { get; } = new List<string>();
		protected internal override void FromExpression(string json)
		{
			Patterns.AddNotEmptyItems(json);
		}

		protected internal override string BuildParametersJson()
		{
			return Patterns.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			if (Patterns.Count == 0)
			{
				return values.LastOrDefault();
			}
			var lastRegexes = Patterns.ToRegex();
			return values.LastOrDefault(value =>
				{
					return !string.IsNullOrEmpty(value) &&
							lastRegexes.Any(r => r.IsMatch(value));
				});
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			if (Patterns.Count == 0)
			{
				return elements.LastOrDefault();
			}

			return elements.LastOrDefault(element => element != null && Patterns.Any(element.ElementMatches));
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			if (Patterns.Count == 0)
			{
				return elements.LastOrDefault();
			}

			return elements.LastOrDefault(element => element != null && Patterns.Any(element.ElementMatches));
		}
	}
}