using Mindscan.Media.HtmlParser.Core.Commands;
using Mindscan.Media.HtmlParser.Core.Helpers;
using System.Collections.Generic;

namespace Mindscan.Media.HtmlParser.Builder
{
	internal sealed class PipelineSupportedCommands : IPipelineSupportedCommands
	{
		public IPipelineCommand Const(params string[] values)
		{
			var result = new Const();
			result.Values.AddNotEmptyItems(values);
			return result;
		}

		public IPipelineCommand Css()
		{
			return new Css();
		}

		public IPipelineCommand XPath()
		{
			return new XPath();
		}

		public IPipelineCommand Join(string delimiter = null)
		{
			return new Join
			{
				Delimiter = delimiter
			};
		}

		public IPipelineCommand Remove(params string[] patterns)
		{
			var result = new Remove();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Skip(params string[] patterns)
		{
			var result = new Skip();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Take(int count = 1)
		{
			return new Take
			{
				Count = count < 1 ? 1 : count
			};
		}

		public IPipelineCommand TakeFirst(params string[] patterns)
		{
			var result = new TakeFirst();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand TakeWhile(params string[] patterns)
		{
			var result = new TakeWhile();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand BreakWhen(params string[] patterns)
		{
			var result = new BreakWhen();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Node(params string[] patterns)
		{
			var result = new Node();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Nodes(params string[] patterns)
		{
			var result = new Nodes();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Text(params string[] skipPatterns)
		{
			var result = new Text();
			result.SkipPatterns.AddNotEmptyItems(skipPatterns);
			return result;
		}

		public IPipelineCommand TextLines(IEnumerable<string> skipPatterns = null, IEnumerable<string> newLinePatterns = null)
		{
			var result = new TextLines();
			result.SkipPatterns.AddNotEmptyItems(skipPatterns);
			result.NewLinePatterns.AddNotEmptyItems(newLinePatterns);
			return result;
		}

		public IPipelineCommand Attr(params string[] attributeNames)
		{
			var result = new Attr();
			result.AttributeNames.AddNotEmptyItems(attributeNames);
			return result;
		}

		public IPipelineCommand SkipLast(int count = 1)
		{
			return new SkipLast
			{
				Count = count < 1 ? 1 : count
			};
		}

		public IPipelineCommand ToDate(string cultureName, params string[] formats)
		{
			var result = new ToDate
			{
				CultureName = cultureName
			};
			result.Formats.AddNotEmptyItems(formats);
			return result;
		}

		public IPipelineCommand ToInt()
		{
			return new ToInt();
		}

		public IPipelineCommand Replace(string oldValue, string newValue = "")
		{
			return new Replace
			{
				OldValue = oldValue ?? "",
				NewValue = newValue ?? ""
			};
		}

		public IPipelineCommand Next(params string[] patterns)
		{
			var result = new Next();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Distinct()
		{
			return new Distinct();
		}

		public IPipelineCommand Attrs(params string[] attributeNames)
		{
			var result = new Attrs();
			result.AttributeNames.AddNotEmptyItems(attributeNames);
			return result;
		}

		public IPipelineCommand Format(string pattern)
		{
			return new Format
			{
				Pattern = pattern
			};
		}

		public IPipelineCommand Match(string[] patterns, string groupName = "")
		{
			var result = new Match
			{
				GroupName = groupName
			};
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Matches(string[] patterns, string groupName = "")
		{
			var result = new Matches
			{
				GroupName = groupName
			};
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand Json()
		{
			return new Json();
		}

		public IPipelineCommand TakeLast(params string[] patterns)
		{
			var result = new TakeLast();
			result.Patterns.AddNotEmptyItems(patterns);
			return result;
		}

		public IPipelineCommand SkipWithText(IEnumerable<string> skipPatterns = null, IEnumerable<string> textPatterns = null)
		{
			var result = new SkipWithText();
			result.SkipPatterns.AddNotEmptyItems(skipPatterns);
			result.TextPatterns.AddNotEmptyItems(textPatterns);
			return result;
		}

		public IPipelineCommand TakeWithText(IEnumerable<string> skipPatterns = null, IEnumerable<string> textPatterns = null)
		{
			var result = new TakeWithText();
			result.SkipPatterns.AddNotEmptyItems(skipPatterns);
			result.TextPatterns.AddNotEmptyItems(textPatterns);
			return result;
		}

		public IPipelineCommand Unescape()
		{
			return new Unescape();
		}

		public IPipelineCommand ToFakeUrl()
		{
			return new ToFakeUrl();
		}

		public IPipelineCommand Decode()
		{
			return new Decode();
		}

		public IPipelineCommand Nop()
		{
			return new Nop();
		}

		public IPipelineCommand Split(string pattern)
		{
			return new Split
			{
				Pattern = pattern
			};
		}

		public IPipelineCommand ToUrl()
		{
			return new ToUrl();
		}
	}
}