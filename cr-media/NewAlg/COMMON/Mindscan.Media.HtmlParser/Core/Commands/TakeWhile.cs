using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.TakeWhile)]
	internal class TakeWhile : CommandBase<IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>>
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
			var breakRegexes = Patterns.ToRegex();
			if (breakRegexes.Count > 0)
				return values.TakeWhile(x => breakRegexes.Any(r => r.IsMatch(x))).ToResult();
			return null;
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			if (Patterns.Count > 0)
			{
				return elements
					.TakeWhile(element => Patterns.Any(element.ElementMatches))
					.ToResult();
			}
			return null;
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			if (Patterns.Count > 0)
			{
				return elements
					.TakeWhile(element => Patterns.Any(element.ElementMatches))
					.ToResult();
			}
			return null;
		}
	}
}