using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class ToDateTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			"waffle",
			DateTimeOffset.Now,
			null,
			typeof(AttrTests),
			new[] { 1, 2, 3 }
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.ToDate("ru");

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("07.06.2018 17:45:07", "2018-06-07T17:45:07+03:00", null)]
		[TestCase("2017-01-12T00:00:00+0300", "2017-01-12T00:00:00+0300", null)]
		[TestCase("вторник, 15 мая 2018 г.", "2018-05-15+03:00", "dddd, d MMMM yyyy г.")]
		[TestCase("28 августа 2017 в 14:25", "2017-08-28T14:25+03:00", "d MMMM yyyy в HH:mm")]
		[TestCase("18.04.2018 09:20", "18.04.2018 09:20:00 +03:00", "d.MM.yyyy HH:mm")]
		[TestCase("09:20", "09:20:00+03:00", "dddd dd MMMM yyyy HH:mm", "dd.MM HH:mm", "HH:mm")]
		[TestCase("9.12.2020 3:58", "03:58:00+03:00", "d.MM.yyyy HH:mm")] //Не будет проходить до 03:58
		[TestCase("Tuesday, 4.7 2017, 5:55", "04 July 2017 05:55", "dddd, d.M yyyy, H: mm")]
		[TestCase("14 Ноября 2017 15:59", "14 November 2017 15:59")]
		[TestCase("Tuesday, 4.7 2017, 5:55", "04 July 2017 05:55", "dddd, d.M yyyy, H:mm")]
		public void TakesString(string date, string outcome, params string[] formats)
		{
			//Arrange
			var command = SupportedCommands.ToDate("ru", formats);
			var pattern = DateTimeOffset.Parse(outcome);

			//Act
			var result = (DateTimeOffset)command.Run(GetContext(), date);

			//Assert
			result.Should().Be(pattern);
		}


		static object[] _casesEn =
		{
			new object[]
			{
				"5 years, 3 months, 2 weeks, 5 days ago at 16:15:21",
				DateTime.Now.Date.AddYears(-5).AddMonths(-3).AddDays(-14).AddDays(-5),
				new TimeSpan(16, 15, 21)
			},
			new object[] { "today at 9:00", DateTime.Now.Date, new TimeSpan(9, 0, 0) }, // до 9 утра может не проходить
			new object[] { "yesterday 21:30", DateTime.Now.Date.AddDays(-1), new TimeSpan(21, 30, 0) },
			new object[] { "about 5 seconds ago", DateTime.Now.Date, DateTime.Now.AddSeconds(-5).TimeOfDay },
			new object[] { "5 seconds ago", DateTime.Now.Date, DateTime.Now.AddSeconds(-5).TimeOfDay },
			new object[] { "5 minutes ago", DateTime.Now.Date, DateTime.Now.AddMinutes(-5).TimeOfDay },
			new object[] { "16 mins.", DateTime.Now.Date, DateTime.Now.AddMinutes(-16).TimeOfDay },
			new object[] { "7 hours ago", DateTime.Now.Date, DateTime.Now.AddHours(-7).TimeOfDay },
			new object[] { "5 h., 11 m. ago", DateTime.Now.Date, DateTime.Now.AddHours(-5).AddMinutes(-11).TimeOfDay },
			new object[] { "5 days ago", DateTime.Now.Date.AddDays(-5), TimeSpan.Zero },
			new object[] { "7 days ago at 10:00", DateTime.Now.Date.AddDays(-7), new TimeSpan(10, 0 , 0) },
			new object[] { "day ago at 12:13:14", DateTime.Now.Date.AddDays(-1), new TimeSpan(12, 13, 14) },
			new object[] { "1 w., 2 d. ago", DateTime.Now.Date.AddDays(-9), TimeSpan.Zero },
			new object[] { "2 w. ago", DateTime.Now.Date.AddDays(-14), TimeSpan.Zero },
			new object[] { "month ago", DateTime.Now.Date.AddMonths(-1), TimeSpan.Zero },
			new object[] { "2 months ago", DateTime.Now.Date.AddMonths(-2), TimeSpan.Zero },
			new object[] { "about 5 years ago", DateTime.Now.Date.AddYears(-5), TimeSpan.Zero },
			new object[] { "3 years ago", DateTime.Now.Date.AddYears(-3), TimeSpan.Zero },
			new object[]
			{
				"5 years, 3 months, 2 weeks, 5 days ago at 16:15:21",
				DateTime.Now.Date.AddYears(-5).AddMonths(-3).AddDays(-19), new TimeSpan(16, 15, 21)
			}
		};
		[TestCaseSource(nameof(_casesEn))]
		public void TakesHumanizedStringEn(string date, DateTime datePart, TimeSpan timePart)
		{
			//Arrange
			var command = SupportedCommands.ToDate("en");
			var time = new DateTimeOffset(
				datePart.Year,
				datePart.Month,
				datePart.Day,
				timePart.Hours,
				timePart.Minutes,
				timePart.Seconds, DateTimeOffset.Now.Offset);

			//Act
			var result = (DateTimeOffset)command.Run(GetContext(), date);

			//Assert
			result.Should().BeCloseTo(time, new TimeSpan(0, 0, 4));
		}

		[TestCase(" ")]
		[TestCase("qwesecqwe")]
		[TestCase("secqwe")]
		[TestCase("qwesec")]
		[TestCase("qweminqwe")]
		[TestCase("minqwe")]
		[TestCase("qwemin")]
		[TestCase("qwehourqwe")]
		[TestCase("hourqwe")]
		[TestCase("qwehour")]
		[TestCase("qwedayqwe")]
		[TestCase("dayqwe")]
		[TestCase("qweday")]
		[TestCase("qweweekqwe")]
		[TestCase("weekqwe")]
		[TestCase("qweweek")]
		[TestCase("qweyearqwe")]
		[TestCase("yearqwe")]
		[TestCase("qweyear")]
		public void IncorrectStringEn(string date)
		{
			//Arrange
			var command = SupportedCommands.ToDate("en");

			//Act
			var result = command.Run(GetContext(), date);

			//Assert
			result.Should().BeNull();
		}

		private static object[] _casesRu =
		{
			new object[] { "1 день ago", DateTime.Now.Date.AddDays(-1), TimeSpan.Zero },
			new object[] { "2 дня ago", DateTime.Now.Date.AddDays(-2), TimeSpan.Zero },
			new object[] { "сегодня в 9:00", DateTime.Now.Date, new TimeSpan(9, 0, 0) }, //Может не проходить до 9 утра
			new object[] { "вчера 21:30", DateTime.Now.Date.AddDays(-1), new TimeSpan(21, 30, 0) },
			new object[] { "5 минут назад", DateTime.Now.Date, DateTime.Now.AddMinutes(-5).TimeOfDay},
			new object[] { "16 мин.", DateTime.Now.Date, DateTime.Now.AddMinutes(-16).TimeOfDay },
			new object[] { "7 часов назад", DateTime.Now.Date, DateTime.Now.AddHours(-7).TimeOfDay },
			new object[] { "2 ч. назад", DateTime.Now.Date, DateTime.Now.AddHours(-2).TimeOfDay },
			new object[] { "4 час., 11 мин. назад", DateTime.Now.Date, DateTime.Now.AddHours(-4).AddMinutes(-11).TimeOfDay},
			new object[] { "5 дней назад", DateTime.Now.Date.AddDays(-5), TimeSpan.Zero},
			new object[] { "7 дней назад в 10:00", DateTime.Now.Date.AddDays(-7), new TimeSpan(10, 0, 0) },
			new object[] { "день назад в 12:13:14", DateTime.Now.Date.AddDays(-1), new TimeSpan(12, 13, 14) },
			new object[] { "3 дня назад", DateTime.Now.Date.AddDays(-3), TimeSpan.Zero },
			new object[] { "21 день назад", DateTime.Now.Date.AddDays(-21), TimeSpan.Zero },
			new object[] { "1 неделя, 2 дн. назад", DateTime.Now.Date.AddDays(-9), TimeSpan.Zero },
			new object[] { "2 нед. назад", DateTime.Now.Date.AddDays(-14), TimeSpan.Zero },
			new object[] { "в прошлом месяце", DateTime.Now.Date.AddMonths(-1), TimeSpan.Zero },
			new object[] { "3 мес назад", DateTime.Now.Date.AddMonths(-3), TimeSpan.Zero },
			new object[] { "в прошлом году", DateTime.Now.Date.AddYears(-1), TimeSpan.Zero },
			new object[] { "3 года назад", DateTime.Now.Date.AddYears(-3), TimeSpan.Zero },
			new object[] { "3 года/лет, 1 месяц назад", DateTime.Now.Date.AddYears(-3).AddMonths(-1), TimeSpan.Zero },
			new object[] { "5 лет назад", DateTime.Now.Date.AddYears(-5), TimeSpan.Zero },
			new object[] {
				"5 лет, 3 месяца, 2 недели, 5 дней назад в 6:15:21",
				DateTime.Now.Date.AddYears(-5).AddMonths(-3).AddDays(-19), new TimeSpan(6, 15, 21)
			},
		};
		[TestCaseSource(nameof(_casesRu))]
		public void TakesHumanizedStringRu(string date, DateTime datePart, TimeSpan timePart)
		{
			//Arrange
			var command = SupportedCommands.ToDate("ru");
			var time = new DateTimeOffset(
				datePart.Year,
				datePart.Month,
				datePart.Day,
				timePart.Hours,
				timePart.Minutes,
				timePart.Seconds, DateTimeOffset.Now.Offset);

			//Act
			var result = (DateTimeOffset)command.Run(GetContext(), date);

			//Assert
			result.Should().BeCloseTo(time, new TimeSpan(0, 0, 4));
		}

		[TestCase(" ")]
		[TestCase("парсеки")]
		[TestCase("секрет")]
		[TestCase("отсек")]
		[TestCase("административный")]
		[TestCase("минимум")]
		[TestCase("админ")]
		[TestCase("счастье")]
		[TestCase("часовой")]
		[TestCase("тотчас")]
		[TestCase("сиденье")]
		[TestCase("деньги")]
		[TestCase("полдень")]
		[TestCase("дно")]
		[TestCase("одно")]
		[TestCase("понедельник")]
		[TestCase("недуг")]
		[TestCase("йцунед")]
		[TestCase("погодки")]
		[TestCase("годный")]
		[TestCase("йцугод")]
		public void IncorrectStringRu(string date)
		{
			//Arrange
			var command = SupportedCommands.ToDate("ru");

			//Act
			var result = command.Run(GetContext(), date);

			//Assert
			result.Should().BeNull();
		}

		[TestCase(1511003520, "2017-11-18T11:12:00+00:00")]
		public void TakesInt(int unixTimestamp, string outcome)
		{
			//Arrange 
			var command = SupportedCommands.ToDate("ru");

			//Act
			var result = (DateTimeOffset)command.Run(GetContext(), unixTimestamp);

			//Assert
			result.Should().Be(DateTimeOffset.Parse(outcome));
		}
	}
}