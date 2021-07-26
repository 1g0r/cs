using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Node)]
	internal class Node : CommandBase<IParentNode, XNode, JToken>
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

		protected override object Execute(ParserContext context, IParentNode parentNode)
		{
			foreach (var selector in Patterns)
			{
				var result = parentNode.QuerySelector(selector);
				if (result == null)
					continue;
				return result;
			}
			return null;
		}

		protected override object Execute(ParserContext context, XNode node)
		{
			foreach (var xPath in Patterns)
			{
				var element = node.XPathSelectElement(xPath);
				if (element != null && !element.IsEmpty)
				{
					return element;
				}
			}
			return null;
		}

		protected override object Execute(ParserContext context, JToken parent)
		{
			foreach (var jPath in Patterns)
			{
				var token = parent.SelectToken(jPath);
				if (token != null)
				{
					return token;
				}
			}
			return null;
		}
	}
}