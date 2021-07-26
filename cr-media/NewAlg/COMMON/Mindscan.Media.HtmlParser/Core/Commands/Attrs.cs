using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.Helpers;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Attrs)]
	internal class Attrs : Attr
	{
		protected override object Execute(ParserContext context, IElement element)
		{
			var values = AttributeNames
				.Where(x => !string.IsNullOrWhiteSpace(element.Attributes[x.ToLower()]?.Value))
				.Select(x => GetAttribute(x.ToLower(), element).ClearString())
				.Where(x => !string.IsNullOrWhiteSpace(x));

			return values.ToResult();
		}

		protected override object Execute(ParserContext context, XElement element)
		{
			var values = AttributeNames
				.Select(x => element.Attribute(x.ToLower())?.Value.ClearString())
				.Where(x => !string.IsNullOrWhiteSpace(x));

			return values.ToResult();
		}
	}
}