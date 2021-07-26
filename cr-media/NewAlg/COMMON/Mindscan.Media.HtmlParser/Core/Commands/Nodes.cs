using System;
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
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Nodes)]
	internal class Nodes : CommandBase<IParentNode, XNode, JToken>
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
			return GetNodes(parent.QuerySelectorAll).ToResult();
		}

		protected override object Execute(ParserContext context, XNode parent)
		{
			return GetNodes(parent.XPathSelectElements).ToResult();
		}

		protected override object Execute(ParserContext context, JToken parent)
		{
			return GetNodes(parent.SelectTokens).ToResult();
		}

		private IEnumerable<TResult> GetNodes<TResult>(Func<string, IEnumerable<TResult>> getter)
		{
			var foundElements = new HashSet<TResult>(); //Distinct
			foreach (var selector in Patterns)
			{
				foreach (var element in getter(selector))
				{
					if (element != null && foundElements.Add(element))
					{
						yield return element;
					}
				}
			}
		}
	}
}