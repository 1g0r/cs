using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	/// <summary>
	/// Удаляет элементы из HTML или XML документа.
	/// </summary>
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Remove)]
	internal class Remove : CommandBase<IParentNode, XNode>
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

		protected override object Execute(ParserContext context, IParentNode parent)
		{
			foreach (var selector in Patterns)
			{
				foreach (var element in parent.QuerySelectorAll(selector))
				{
					element.Remove();
				}
			}
			return parent;
		}

		protected override object Execute(ParserContext context, XNode parent)
		{
			foreach (var pattern in Patterns)
			{
				foreach (var xElement in parent.XPathSelectElements(pattern))
				{
					xElement.Remove();
				}
			}
			return parent;
		}
	}
}