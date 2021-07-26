using System;
using System.Collections.Generic;
using System.Globalization;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.ToDate)]
	internal class ToDate : CommandBase<string, int>
	{
		[JsonProperty(Order = 1)]
		public string CultureName { get; set; }

		[JsonProperty(Order = 2)]
		public List<string> Formats { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			var patterns = JsonConvert.DeserializeObject<List<string>>(json);
			if (patterns != null)
			{
				if (patterns.Count > 0)
				{
					CultureName = patterns[0];
					patterns.RemoveAt(0);
				}
				Formats.AddRange(patterns);
			}
		}

		protected internal override string BuildParametersJson()
		{
			var parameters = new List<string>();
			if (!string.IsNullOrWhiteSpace(CultureName))
			{
				parameters.Add(CultureName);
			}
			parameters.AddRange(Formats);
			return parameters.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			DateTimeOffset result;
			foreach (var format in Formats)
			{
				if (DateTimeOffset.TryParseExact(value, format, Culture, DateTimeStyles.AssumeLocal, out result))
				{
					return CheckNotFutureDate(result);
				}
			}
			if (DateTimeOffset.TryParse(value, out result))
			{
				return CheckNotFutureDate(result);
			}
			if (DateTimeOffsetHelper.TryParse(value, Culture, out result))
			{
				return CheckNotFutureDate(result);
			}
			return null;
		}

		protected override object Execute(ParserContext context, int value)
		{
			if(value > 0)
				return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.UtcNow.Offset).AddSeconds(value);
			return null;
		}

		private CultureInfo Culture
		{
			get
			{
				if (string.IsNullOrEmpty(CultureName))
					return CultureInfo.GetCultureInfo("ru");
				return CultureInfo.GetCultureInfo(CultureName);
			}
		}

		private static DateTimeOffset CheckNotFutureDate(DateTimeOffset result)
		{
			var now = DateTimeOffset.Now;
			if (result > now)
			{
				var diff = now - result;
				var newDate = result.Date.Add(diff).Date.Add(result.TimeOfDay);
				return new DateTimeOffset(newDate, result.Offset);
			}
			return result;
		}
	}
}