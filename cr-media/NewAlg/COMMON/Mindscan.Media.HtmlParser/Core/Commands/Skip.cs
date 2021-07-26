using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Skip)]
	internal class Skip : CommandBase<IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>>
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
			var skipRegexes = Patterns.ToRegex();
			return values
				.Where(x => !string.IsNullOrEmpty(x) && !skipRegexes.Any(r => r.IsMatch(x)))
				.ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			return elements
				.Where(element => !Patterns.Any(element.ElementMatches))
				.ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			return elements
				.Where(element => !Patterns.Any(element.ElementMatches))
				.ToResult();
		}
	}
}