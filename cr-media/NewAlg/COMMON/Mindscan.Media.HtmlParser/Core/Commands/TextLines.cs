using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.TextLines)]
	internal class TextLines : CommandBase<IEnumerable<string>, IElement, IEnumerable<IElement>, XElement, IEnumerable<XElement>, IEnumerable<JToken>>
	{
		[JsonProperty(Order = 1)]
		public List<string> SkipPatterns { get; } = new List<string>();

		[JsonProperty(Order = 2)]
		public List<string> NewLinePatterns { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			var patterns = JsonConvert.DeserializeObject<List<string>[]>(json);
			if (patterns != null && patterns.Length > 0)
			{
				if (patterns.Length > 0)
				{
					var newLinePatterns = patterns[0];
					NewLinePatterns.AddRange(newLinePatterns);
				}
				if (patterns.Length > 1)
				{
					var skipPatterns = patterns[0];
					SkipPatterns.AddRange(skipPatterns);
				}
			}
		}

		protected internal override string BuildParametersJson()
		{
			return new List<List<string>> { SkipPatterns, NewLinePatterns }.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			return values;
		}

		protected override object Execute(ParserContext context, IElement element)
		{
			return GetEnumerable(new List<IElement> { element })
				.Distinct()
				.ToResult();
		}
		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			return GetEnumerable(elements)
				.Distinct()
				.ToResult();
		}

		protected override object Execute(ParserContext context, XElement element)
		{
			return GetEnumerable(new List<XElement> { element })
				.Distinct()
				.ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			return GetEnumerable(elements)
				.Distinct()
				.ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<JToken> tokens)
		{
			var values = GetEnumerable(tokens).Distinct().ToResult();
			return values;
		}

		private IEnumerable<string> GetEnumerable(IEnumerable<IElement> elements)
		{
			if (elements == null)
				yield break;

			var skipSelectors = SkipPatterns.ToSkipPatterns();
			foreach (var element in elements)
			{
				// Skip self
				if (skipSelectors.Any(element.ElementMatches))
					continue;

				var lines = GetLinesOfText(element, skipSelectors, NewLinePatterns.ToNewLinePatterns());
				foreach (var line in lines)
				{
					yield return line;
				}
			}
		}

		private IEnumerable<string> GetEnumerable(IEnumerable<XElement> elements)
		{
			if (elements == null)
				yield break;

			var skipSelectors = SkipPatterns.ToSkipPatterns();
			foreach (var element in elements)
			{
				//Skip self
				if (skipSelectors.Any(element.ElementMatches))
					continue;

				var lines = GetLinesOfText(element, skipSelectors, NewLinePatterns.ToNewLinePatterns());
				foreach (var line in lines)
				{
					yield return line;
				}
			}
		}

		private static IEnumerable<string> GetLinesOfText(
			INode node,
			List<string> elementsToSkip,
			List<string> newLineElements,
			bool first = true,
			StringBuilder builder = null)
		{
			if (node == null)
				yield break;

			if (builder == null)
			{
				builder = new StringBuilder();
			}
			if (node.NodeType == NodeType.Text)
			{
				builder.AppendTail(node.GetStringFromTextNode());
			}
			foreach (var child in node.ChildNodes)
			{
				// Each table row should be as single paragraph
				if (child is IHtmlTableRowElement)
				{
					if (builder.Length > 0)
					{
						yield return builder.ToString();
						builder.Clear();
					}

					builder.AppendTail(child.GetText(elementsToSkip));
					if (builder.Length > 0)
					{
						yield return builder.ToString();
						builder.Clear();
					}

					continue;
				}

				// Skip element if it should be skipped
				var childElement = child as IElement;
				if (childElement != null && elementsToSkip.Any(childElement.ElementMatches))
				{
					if (IsNewLineElement(childElement, newLineElements, builder))
					{
						yield return builder.ToString();
						builder.Clear();
					}
					continue;
				}

				if (child.NodeType == NodeType.Text)
				{
					builder.AppendTail(child.GetStringFromTextNode());
				}

				if (IsNewLineElement(childElement, newLineElements, builder))
				{
					yield return builder.ToString();
					builder.Clear();
				}

				if (child.HasChildNodes)
				{
					foreach (var line in GetLinesOfText(child, elementsToSkip, newLineElements, false, builder))
					{
						yield return line;
					}
					if (IsNewLineElement(childElement, newLineElements, builder))
					{
						yield return builder.ToString();
						builder.Clear();
					}
				}
			}
			if (first && builder.Length > 0)
				yield return builder.ToString();
		}

		private static bool IsNewLineElement(IElement element, List<string> newLineElements, StringBuilder result)
		{
			if (element == null || newLineElements == null || newLineElements.Count == 0)
				return false;
			return newLineElements.Any(element.ElementMatches) && result.Length > 0;
		}

		private static IEnumerable<string> GetLinesOfText(
			XNode node,
			List<string> elementsToSkip,
			List<string> newLineElements,
			bool first = true,
			StringBuilder builder = null)

		{
			if (node == null || node.NodeType == XmlNodeType.CDATA || node.NodeType == XmlNodeType.Comment)
				yield break;

			if (builder == null)
			{
				builder = new StringBuilder();
			}

			if (node.NodeType == XmlNodeType.Text)
			{
				builder.AppendTail(node.GetStringFromTextNode());
			}

			var parent = node as XElement;
			if (parent == null)
				yield break;

			foreach (var child in parent.Nodes())
			{
				// Each table row should be as single paragraph
				if (child.ElementMatches("tr"))
				{
					if (builder.Length > 0)
					{
						yield return builder.ToString();
						builder.Clear();
					}

					builder.AppendTail(child.GetText(elementsToSkip));
					if (builder.Length > 0)
					{
						yield return builder.ToString();
						builder.Clear();
					}

					continue;
				}

				// Skip element if it should be skipped
				var childElement = child as XElement;
				if (childElement != null && elementsToSkip.Any(childElement.ElementMatches))
				{
					if (IsNewLineElement(childElement, newLineElements, builder))
					{
						yield return builder.ToString();
						builder.Clear();
						continue;
					}
				}

				if (child.NodeType == XmlNodeType.Text)
				{
					builder.AppendTail(child.GetStringFromTextNode());
				}

				if (IsNewLineElement(childElement, newLineElements, builder))
				{
					yield return builder.ToString();
					builder.Clear();
				}

				if (childElement != null && childElement.Nodes().Any())
				{
					foreach (var line in GetLinesOfText(child, elementsToSkip, newLineElements, false, builder))
					{
						yield return line;
					}
					if (IsNewLineElement(childElement, newLineElements, builder))
					{
						yield return builder.ToString();
						builder.Clear();
					}
				}
			}
			if (first && builder.Length > 0)
				yield return builder.ToString();

		}

		private static bool IsNewLineElement(XNode element, List<string> newLineElements, StringBuilder result)
		{
			if (element == null || newLineElements == null || newLineElements.Count == 0)
				return false;
			return newLineElements.Any(element.ElementMatches) && result.Length > 0;
		}

		private IEnumerable<string> GetEnumerable(IEnumerable<JToken> tokens)
		{
			var skipRegexes = SkipPatterns.ToRegex();
			foreach (var token in tokens)
			{
				var result = token.Value<string>();
				if (!string.IsNullOrWhiteSpace(result))
				{
					if (skipRegexes.Count > 0 && skipRegexes.Any(r => r.IsMatch(result)))
						continue;
					yield return result;
				}
			}
		}
	}
}