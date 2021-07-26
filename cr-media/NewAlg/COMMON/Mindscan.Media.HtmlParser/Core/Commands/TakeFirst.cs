using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.TakeFirst)]
	internal class TakeFirst : CommandBase<IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>>
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
			var firstRegexes = Patterns.ToRegex();

			if (Patterns.Count == 0)
			{
				return values.FirstOrDefault();
			}

			return values.FirstOrDefault(value =>
				{
					return !string.IsNullOrEmpty(value) &&
							firstRegexes.Any(r => r.IsMatch(value));
				}).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			if (Patterns.Count == 0)
			{
				return elements.FirstOrDefault();
			}

			return elements
				.FirstOrDefault(element => element != null && Patterns.Any(element.ElementMatches));
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			if (Patterns.Count == 0)
			{
				return elements.FirstOrDefault();
			}

			return elements.FirstOrDefault(element => element != null && Patterns.Any(element.ElementMatches));
		}
	}
}