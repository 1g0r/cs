using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Format)]
	internal class Format : CommandBase<string, IEnumerable<string>, IEnumerable<object>>
	{
		[JsonProperty(Order = 1)]
		public string Pattern { get; set; }
		protected internal override void FromExpression(string json)
		{
			Pattern = JsonConvert.DeserializeObject<List<string>>(json)?.FirstOrDefault();
		}

		protected internal override string BuildParametersJson()
		{
			return Pattern.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			return FormatString(values).ToResult();
		}

		protected override object Execute(ParserContext context, string value)
		{
			return FormatString(new[] { value }).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<object> data)
		{
			try
			{
				return string.Format(Pattern, data.ToArray());
			}
			catch (FormatException fe)
			{
				return Pattern;
			}
		}

		private string FormatString(IEnumerable<string> values)
		{
			if (string.IsNullOrWhiteSpace(Pattern))
				return null;

			try
			{
				return string.Format(Pattern, values.ToArray<object>());
			}
			catch (FormatException fe)
			{
				return Pattern;
			}
		}
	}
}