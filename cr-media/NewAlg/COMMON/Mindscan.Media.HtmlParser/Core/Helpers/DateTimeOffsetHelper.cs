using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class DateTimeOffsetHelper
	{
		private static Regex TimeRegex => @"\d{1,2}:\d{2}(:\d{2})?".CreateRegex();

		public static bool TryParse(string text, CultureInfo culture, out DateTimeOffset value)
		{
			DateTime parsed = DateTime.Now;
			parsed = parsed.AddMilliseconds(-parsed.Millisecond);
			value = DateTimeOffset.MinValue;

			bool success = false;

			Match match;
			if (TryMatch(culture.SecondsAgoRegex(), text, out match))
			{
				var secondsAgo = GetValueAgo(culture, match);

				parsed = parsed.AddSeconds(-secondsAgo);
				success = true;
			}
			if (TryMatch(culture.MinutesAgoRegex(), text, out match))
			{
				int minutesAgo = GetValueAgo(culture, match);

				parsed = parsed.AddMinutes(-minutesAgo);
				success = true;
			}
			if (TryMatch(culture.HoursAgoRegex(), text, out match))
			{
				int hoursAgo = GetValueAgo(culture, match);

				parsed = parsed.AddHours(-hoursAgo);
				success = true;
			}
			if (TryMatch(culture.YearsAgoRegex(), text, out match))
			{
				int yearsAgo = GetValueAgo(culture, match);

				parsed = parsed.Date.AddYears(-yearsAgo);
				success = true;
			}
			if (TryMatch(culture.MonthsAgoRegex(), text, out match))
			{
				int monthsAgo = GetValueAgo(culture, match);

				parsed = parsed.Date.AddMonths(-monthsAgo);
				success = true;
			}
			if (TryMatch(culture.DaysAgoRegex(), text, out match))
			{
				int daysAgo = GetValueAgo(culture, match);

				parsed = parsed.Date.AddDays(-daysAgo);
				success = true;
			}
			if (TryMatch(culture.WeeksAgoRegex(), text, out match))
			{
				int weeksAgo = GetValueAgo(culture, match);

				parsed = parsed.Date.AddDays(-(weeksAgo * 7));
				success = true;
			}
			if (TryMatch(culture.YesterdayRegex(), text, out match))
			{
				parsed = DateTime.Now.Date.AddDays(-1);
				success = true;
			}
			else if (TryMatch(culture.TodayRegex(), text, out match))
			{
				parsed = DateTime.Now.Date;
				success = true;
			}

			if (success)
			{
				parsed = ParseTime(text, culture, parsed);
				value = new DateTimeOffset(DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified), TimeZoneInfo.Local.BaseUtcOffset);
			}

			return success;
		}

		private static int GetValueAgo(IFormatProvider formatProvider, Match match)
		{
			var valueAgo = 1;
			if (!match.Groups["value"].Success)
				return valueAgo;

			var matched = match.Groups["value"].Value;
			int value;
			if (int.TryParse(matched, NumberStyles.Any, formatProvider, out value))
			{
				valueAgo = value;
			}
			return valueAgo;
		}

		private static DateTime ParseTime(string text, IFormatProvider formatProvider, DateTime parsed)
		{
			TimeSpan time;
			if (TryParseTime(text, formatProvider, out time))
			{
				parsed = parsed.Add(time);
			}
			return parsed;
		}

		private static bool TryParseTime(string text, IFormatProvider formatProvider, out TimeSpan time)
		{
			Match matchTime = TimeRegex.Match(text);

			if (!matchTime.Success || !TimeSpan.TryParse(matchTime.Value, formatProvider, out time))
			{
				time = TimeSpan.MinValue;
				return false;
			}

			return true;
		}

		private static bool TryMatch(Regex regex, string text, out Match match)
		{
			match = regex.Match(text);
			return match.Success;
		}
	}
}