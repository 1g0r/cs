using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class CultureHelper
	{
		private static RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
		private const string Default = "Default";

		private static readonly IDictionary<string, Regex> Regexies = new Dictionary<string, Regex>()
		{
			{nameof(SecondsAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:s(ec|econd|econds)*($|\s|,|\.))".CreateRegex() },
			{nameof(SecondsAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:сек(унд|унду|унды|унда)*($|\s|,|\.))".CreateRegex() },

			{nameof(MinutesAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:m(inute|inutes|ins)*($|\s|,|\.))".CreateRegex() },
			{ nameof(MinutesAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:мин(ут|уту|уты|ута)*($|\s|,|\.))".CreateRegex() },

			{nameof(HoursAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:h(our|ours)*($|\s|,|\.))".CreateRegex() },
			{ nameof(HoursAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:ч(ас|ов|а)*($|\s|,|\.))".CreateRegex() },

			{nameof(DaysAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:d(ay|ays)*($|\s|,|\.))".CreateRegex() },
			{ nameof(DaysAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:д(ней|ня|ень|н)*($|\s|,|\.))".CreateRegex() },

			{nameof(WeeksAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:w(eek|eeks)*($|\s|,|\.))".CreateRegex() },
			{ nameof(WeeksAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:нед(ель|елю|ели|еля)*($|\s|,|\.))".CreateRegex() },

			{nameof(MonthsAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:month(s)*($|\s|,|\.))".CreateRegex() },
			{ nameof(MonthsAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:мес(яц|яцев|яца|е)*($|\s|,|\.))".CreateRegex() },

			{nameof(YearsAgoRegex), @"((?'value'\d{1,2}))?(\s+|^)(?:(y|ear|ears)*($|\s|,|\.|\/))".CreateRegex() },
			{ nameof(YearsAgoRegex) + Default, @"((?'value'\d{1,2}))?(\s+|^)(?:(году?|года|лет){1}($|\s|,|\.|\/))".CreateRegex() },

			{nameof(YesterdayRegex), "yesterday".CreateRegex() },
			{ nameof(YesterdayRegex) + Default, @"вчера".CreateRegex() },

			{nameof(TodayRegex), "today".CreateRegex() },
			{ nameof(TodayRegex) + Default, @"(с|c)егодня".CreateRegex() }
		};

		public static Regex SecondsAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(SecondsAgoRegex));
		}

		public static Regex MinutesAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(MinutesAgoRegex));
		}

		public static Regex HoursAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(HoursAgoRegex));
		}

		public static Regex DaysAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(DaysAgoRegex));
		}

		public static Regex WeeksAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(WeeksAgoRegex));
		}

		public static Regex MonthsAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(MonthsAgoRegex));
		}

		public static Regex YearsAgoRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(YearsAgoRegex));
		}

		public static Regex YesterdayRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(YesterdayRegex));
		}

		public static Regex TodayRegex(this CultureInfo culture)
		{
			return GetRegex(culture, nameof(TodayRegex));
		}

		private static Regex GetRegex(CultureInfo culture, string name)
		{
			switch (culture.Name)
			{

				case "en":
					return Regexies[name];
				default:
					return Regexies[name + Default];
			}
		}
	}
}