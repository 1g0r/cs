using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Matches)]
	internal class Matches : CommandBase<string>
	{
		[JsonProperty(Order = 1)]
		public List<string> Patterns { get; } = new List<string>();

		[JsonProperty(Order = 1)]
		public string GroupName { get; set; } = "";

		protected internal override void FromExpression(string json)
		{
			var patterns = JsonConvert.DeserializeObject<List<string>>(json);
			if (patterns != null)
			{
				if (patterns.Count > 0)
				{
					GroupName = patterns[0];
					patterns.RemoveAt(0);
				}

				Patterns.AddRange(patterns);
			}
		}

		protected internal override string BuildParametersJson()
		{
			var parameters = new List<string>
			{
				GroupName ?? string.Empty
			};
			parameters.AddRange(Patterns);
			return parameters.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, string value)
		{
			foreach (var regex in Patterns.ToRegex())
			{
				var result = GetMatches(regex, value).ToList();
				if (result.Any())
					return result.ToResult();
			}
			return null;
		}

		private IEnumerable<string> GetMatches(Regex regex, string value)
		{
			var matches = regex.Matches(value)
				.OfType<System.Text.RegularExpressions.Match>();

			foreach (var match in matches)
			{
				var result = GetValue(match);
				if (result != null)
					yield return result;
			}
		}

		private string GetValue(System.Text.RegularExpressions.Match match)
		{
			if (!string.IsNullOrWhiteSpace(GroupName))
			{
				var group = match.Groups[GroupName];
				if (group.Success)
				{
					return group.Value;
				}
				return null;
			}
			return match.Value;
		}
	}
}