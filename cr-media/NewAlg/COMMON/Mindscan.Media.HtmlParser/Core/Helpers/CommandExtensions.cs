using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using Mindscan.Media.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class CommandExtensions
	{
		public static void AddNotEmptyItems(this List<string> toList, string json)
		{
			var values = JsonConvert
				.DeserializeObject<List<string>>(json)
				?.Where(x => !string.IsNullOrEmpty(x));

			toList?.AddRange(values ?? Enumerable.Empty<string>());
		}

		public static void AddNotEmptyItems(this List<string> toList, IEnumerable<string> values)
		{
			toList?.AddRange(
				values?.Where(x => !string.IsNullOrEmpty(x)) ?? Enumerable.Empty<string>()
			);
		}

		public static List<Regex> ToRegex(this List<string> patterns)
		{
			if (patterns == null || patterns.Count == 0)
				return new List<Regex>();
			var result = new List<Regex>();
			foreach (var pattern in patterns)
			{
				if (!string.IsNullOrEmpty(pattern))
				{
					result.Add(pattern.CreateRegex());
				}
			}
			return result;
		}

		public static bool ElementMatches(this IElement element, string selector)
		{
			try
			{
				return element.Matches(selector);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Unable to match selector {selector}", ex);
			}
		}

		public static bool ElementMatches(this XNode element, string xPath)
		{
			try
			{
				var navigator = element.CreateNavigator();
				return navigator.Matches(xPath);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Unable to match XPath {xPath}", ex);
			}
		}

		public static string GetStringFromTextNode(this INode node)
		{
			if (node == null || node.NodeType != NodeType.Text)
				return string.Empty;

			var rawValue = node.TextContent;

			return rawValue.ClearString();
		}

		public static string GetStringFromTextNode(this XNode node)
		{
			if (node == null || node.NodeType != XmlNodeType.Text)
				return string.Empty;

			var rawValue = node.ToString(SaveOptions.None);

			return rawValue.ClearString();
		}

		public static object ToResult(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			return value;
		}

		public static object ToResult<T>(this IEnumerable<T> values)
		{
			var result = values?.Where(x => x != null).ToList();
			return result?.Count > 0 ? result : null;
		}

		public static List<string> ToNewLinePatterns(this List<string> patterns)
		{
			if (patterns.Count == 0)
			{
				lock (patterns)
				{
					if (patterns.Count == 0)
					{
						patterns.AddRange(new[] { "p", "br", "li", "h1", "h2", "h3", "h4", "h5", "h6" });
					}
				}
			}
			return patterns;
		}

		public static List<string> ToSkipPatterns(this List<string> patterns)
		{
			if (patterns.Count == 0)
			{
				lock (patterns)
				{
					if (patterns.Count == 0)
					{
						patterns.AddRange(new[] { "script", "iframe", "style" });
					}
				}
			}
			return patterns;
		}
	}
}
