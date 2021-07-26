using System.Collections.Generic;

namespace Mindscan.Media.HtmlParser
{
	public static class SupportedNames
	{
		public static class Schema
		{
			public const string Namespace = "schema";
			public const string Simple = "simple";
			public const string Complex = "complex";
			public const string Array = "array";

			public static string GetFullName() => "";
		}

		public static class Expressions
		{
			public const string Namespace = "expression";
			public const string Or = "or";
			public const string And = "and";
			public const string Pipeline = "pipeline";
			public const string For = "for";
		}

		public static class Commands
		{
			public const string Namespace = "command";
			public const string Remove = "remove";
			public const string Skip = "skip";
			public const string TakeWhile = "takeWhile";
			public const string Join = "join";
			public const string Const = "const";
			public const string TakeFirst = "takeFirst";
			public const string Css = "css";
			public const string Node = "node";
			public const string Nodes = "nodes";
			public const string Text = "text";
			public const string TextLines = "textLines";
			public const string Attr = "attr";
			public const string SkipLast = "skipLast";
			public const string ToDate = "toDate";
			public const string XPath = "xPath";
			public const string ToInt = "toInt";
			public const string Replace = "replace";
			public const string BreakWhen = "breakWhen";
			public const string Next = "next";
			public const string Distinct = "distinct";
			public const string Attrs = "attrs";
			public const string Format = "format";
			public const string Match = "match";
			public const string Matches = "matches";
			public const string Json = "json";
			public const string TakeLast = "takeLast";
			public const string SkipWithText = "skipWithText";
			public const string TakeWithText = "takeWithText";
			public const string Unescape = "unescape";
			public const string ToFakeUrl = "toFakeUrl";
			public const string Take = "take";
			public const string Decode = "decode";
			public const string Nop = "nop";
			public const string Split = "split";
			public const string ToUrl = "toUrl";
		}

		public static class Parsers
		{
			public const string Namespace = "parsers";
			public const string Base = "htmlParser";
		}

		public static IEnumerable<string> GetCommandNames()
		{
			var properties = typeof(Commands).GetFields();
			foreach (var fieldInfo in properties)
			{
				if (fieldInfo.Name != nameof(Commands.Namespace))
					yield return (string)fieldInfo.GetValue(null);
			}
		}
	}
}
