using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Next)]
	internal class Next : CommandBase<IElement, XElement>
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

		protected override object Execute(ParserContext context, IElement element)
		{
			foreach (var selector in Patterns)
			{
				var result = Sibling(element, selector);
				if (result == null)
					continue;
				return result;
			}
			return null;
		}

		protected override object Execute(ParserContext context, XElement element)
		{
			foreach (var xPath in Patterns)
			{
				var path = string.IsNullOrWhiteSpace(xPath) ? "" : $"following-sibling::{xPath}";
				var result = element?.XPathSelectElement(path);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		private IElement Sibling(IElement parentNode, string selector)
		{
			var sibling = parentNode?.NextElementSibling;
			while (sibling != null)
			{
				if (sibling.Matches(selector))
					return sibling;
				sibling = sibling.NextElementSibling;
			}
			return null;
		}
	}
}