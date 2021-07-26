using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.TakeWithText)]
	internal class TakeWithText : CommandBase<IEnumerable<IElement>, IEnumerable<XElement>>
	{
		[JsonProperty(Order = 1)]
		public List<string> SkipPatterns { get; } = new List<string>();

		[JsonProperty(Order = 2)]
		public List<string> TextPatterns { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			var patterns = JsonConvert.DeserializeObject<List<string>[]>(json);
			if (patterns != null && patterns.Length > 0)
			{
				if (patterns.Length > 0)
				{
					var newLinePatterns = patterns[0];
					TextPatterns.AddRange(newLinePatterns);
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
			return new List<List<string>> { SkipPatterns, TextPatterns }.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> data)
		{
			var textRegexes = GetTextRegexes();
			return data
				.Where(x =>
				{
					var text = TextHelper.GetText((INode) x, SkipPatterns.ToSkipPatterns());
					return textRegexes.Any(r => r.IsMatch(text));
				})
				.ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> data)
		{
			var textRegexes = GetTextRegexes();
			return data
				.Where(x =>
				{
					var text = x.GetText(SkipPatterns.ToSkipPatterns());
					return textRegexes.Any(r => r.IsMatch(text));
				})
				.ToResult();
		}

		private List<Regex> GetTextRegexes()
		{
			if (TextPatterns.Count == 0)
			{
				TextPatterns.Add("^$");
			}
			return TextPatterns.ToRegex();
		}
	}
}