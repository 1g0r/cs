using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Mindscan.Media.Helpers;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Attr)]
	internal class Attr : CommandBase<IElement, XElement>
	{
		[JsonProperty(Order = 1)]
		public List<string> AttributeNames { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			var names = JsonConvert
				.DeserializeObject<List<string>>(json)
				?.Where(x => !string.IsNullOrWhiteSpace(x));
			if (names != null)
			{
				foreach (var name in names)
				{
					AttributeNames.Add(name.ToLower());
				}
			}
		}

		protected internal override string BuildParametersJson()
		{
			return AttributeNames.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IElement element)
		{
			var value = AttributeNames
				.Where(x => !string.IsNullOrWhiteSpace(element.Attributes[x.ToLower()]?.Value))
				.Select(x => GetAttribute(x.ToLower(), element))
				.FirstOrDefault();

			return value.ClearString().ToResult();
		}

		protected override object Execute(ParserContext context, XElement element)
		{
			var value = AttributeNames
				.Select(x => element.Attribute(x.ToLower())?.Value)
				.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

			return value.ClearString().ToResult();
		}

		protected static string GetAttribute(string name, IElement element)
		{
			var ownAttribute = element.Attributes[name]?.Value;
			if (!string.IsNullOrWhiteSpace(ownAttribute) && IsUrlAttribute(name))
			{
				Url url = new Url(element.BaseUrl, ownAttribute);
				if (url.IsInvalid)
					return string.Empty;
				return url.Href;
			}
			return ownAttribute;
		}

		private static bool IsUrlAttribute(string name)
		{
			return !string.IsNullOrEmpty(name) && (
					   name.Equals(AngleSharp.Html.AttributeNames.Href, StringComparison.InvariantCultureIgnoreCase) ||
					   name.Equals(AngleSharp.Html.AttributeNames.Src, StringComparison.InvariantCultureIgnoreCase)
				   );
		}
	}
}