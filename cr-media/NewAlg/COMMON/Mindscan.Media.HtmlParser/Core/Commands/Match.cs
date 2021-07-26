using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Match)]
	internal class Match : CommandBase<string>
	{
		[JsonProperty(Order = 1)]
		public List<string> Patterns { get; } = new List<string>();

		[JsonProperty(Order = 2)]
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
				var result = MatchRegex(regex, value);
				if (result != null)
					return result;
			}
			return null;
		}

		private object MatchRegex(Regex regex, string value)
		{
			var match = regex.Match(value);
			if (match.Success)
			{
				if (!string.IsNullOrWhiteSpace(GroupName))
				{
					var group = match.Groups[GroupName];
					if (group.Success)
					{
						return group.Value.ToResult();
					}
					return null;
				}
				return match.Value.ToResult();
			}
			return null;
		}
	}
}