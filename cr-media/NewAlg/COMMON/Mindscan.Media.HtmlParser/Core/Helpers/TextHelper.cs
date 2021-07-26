using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class TextHelper
	{
		public static string GetText(this INode node, List<string> skipSelectors)
		{
			if (node.NodeType == NodeType.Text)
			{
				return node.GetStringFromTextNode();
			}

			var result = new StringBuilder();
			foreach (var child in node.ChildNodes)
			{
				var childElement = child as IElement;
				if (childElement != null && skipSelectors.Any(t => childElement.ElementMatches(t)))
					continue;

				if (child.NodeType == NodeType.Text)
				{
					result.AppendTail(child.GetStringFromTextNode());
				}

				if (child.HasChildNodes)
				{
					result.AppendTail(GetText(child, skipSelectors));
				}
			}
			return result.ToString();
		}

		public static string GetText(this XNode node, List<string> xPathList)
		{
			if (node == null || node.NodeType == XmlNodeType.CDATA || node.NodeType == XmlNodeType.Comment)
				return string.Empty;

			if (node.NodeType == XmlNodeType.Text)
			{
				return node.GetStringFromTextNode();
			}

			var parent = node as XElement;
			if (parent == null)
				return "";

			var result = new StringBuilder();
			foreach (var child in parent.Nodes())
			{
				if (xPathList.Any(xPath => child.ElementMatches(xPath)))
					continue;
				if (child.NodeType == XmlNodeType.Text)
				{
					result.AppendTail(child.GetStringFromTextNode());
				}

				var childElement = child as XElement;
				if (childElement != null && childElement.Nodes().Any())
				{
					result.AppendTail(GetText(child, xPathList));
				}
			}
			return result.ToString();
		}

		public static void AppendTail(this StringBuilder value, string tail)
		{
			if (value == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(tail))
			{
				return;
			}
			if (value.Length == 0)
			{
				value.Append(tail);
				return;
			}
			if (IsNotRequireWhiteSpaceSymbol(value[value.Length - 1]))
			{
				value.Append(tail);
				return;
			}
			if (char.IsPunctuation(tail[0]) && !IsQuotes(tail[0]) || QuotesNotRequireWhiteSpace(value.ToString(), tail))
			{
				value.Append(tail);
				return;
			}
			value.Append(" " + tail);
		}

		private static bool QuotesNotRequireWhiteSpace(string value, string tail)
		{
			if (value.Length == 1 && IsQuotes(value[0]))
				return true;

			if (IsQuotes(tail[0]) && tail.Length > 1)
			{
				return tail[1] == ' ';
			}
			if (value.Length > 1 && IsQuotes(value[value.Length - 1]))
			{
				return value[value.Length - 2] == ' ';
			}
			return false;
		}

		private static bool IsNotRequireWhiteSpaceSymbol(char symbol)
		{
			return symbol == ' ' || symbol == '\n' || symbol == '\r' || symbol == '\'';
		}

		private static bool IsQuotes(char symbol)
		{
			return symbol == 34 || symbol == 171 || symbol == 187;
		}

	}

}