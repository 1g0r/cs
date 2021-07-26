using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mindscan.Media.Helpers;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Replace)]
	internal class Replace : CommandBase<string, IEnumerable<string>>
	{
		[JsonProperty(Order = 1)]
		public string OldValue { get; set; } = "";

		[JsonProperty(Order = 2)]
		public string NewValue { get; set; } = "";

		protected internal override void FromExpression(string json)
		{
			var patterns = JsonConvert.DeserializeObject<List<string>>(json);
			if (patterns != null)
			{
				if (patterns.Count > 0)
				{
					OldValue = patterns[0];
				}
				if (patterns.Count > 1)
				{
					NewValue = patterns[1];
				}
			}
		}

		protected internal override string BuildParametersJson()
		{
			return new[] { OldValue, NewValue }.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, string value)
		{
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(OldValue))
			{
				var oldRegex = OldValue.CreateRegex();
				var result = oldRegex.Replace(value, NewValue ?? "").ClearString();
				return (result ?? "").ToResult();
			}
			return ReplaceSubstring(value).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			var result = new List<string>();
			foreach (var value in values)
			{
				var newValue = ReplaceSubstring(value);
				if (!string.IsNullOrEmpty(newValue))
				{
					result.Add(newValue);
				}
			}
			return result.ToResult();
		}

		private string ReplaceSubstring(string value)
		{
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(OldValue))
			{
				var oldRegex = OldValue.CreateRegex();
				var result = oldRegex.Replace(value, NewValue ?? "").ClearString();
				return result ?? "";
			}
			return null;
		}
	}
}