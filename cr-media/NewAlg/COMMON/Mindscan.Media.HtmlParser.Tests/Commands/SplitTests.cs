using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class SplitTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			null,
			"",
			typeof(AttrTests)
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Split("sdfkj");

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}


		[TestCase(
			@"(?:,|\.|\s+и|\s+при\s+участии)\s*",
			"Стив Холланд и Роберта Рэмптон при участии Сьюзан Хиви, Джеффа Мэйсона и Пола Симао. Перевела Вера Сосенкова. Редактор Дмитрий Антонов", "Стив Холланд", "Роберта Рэмптон", "Сьюзан Хиви", "Джеффа Мэйсона", "Пола Симао", "Перевела Вера Сосенкова", "Редактор Дмитрий Антонов")]
		[TestCase(null, "waffle", "waffle")]
		public void SuccessSplit(string pattern, string value, params string[] outcome)
		{
			//Arrange
			var command = SupportedCommands.Split(pattern);

			//Act
			var result = (IEnumerable<string>)command.Run(GetContext(), value);

			//Assert
			result.Should().NotBeEmpty();
			result.Count().Should().Be(outcome.Length);
			result.Should().BeEquivalentTo(outcome);
		}
	}
}