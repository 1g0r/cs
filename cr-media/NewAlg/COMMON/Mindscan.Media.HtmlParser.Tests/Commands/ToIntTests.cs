using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class ToIntTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			"waffle",
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests),
			new[] { 1, 2, 3 }
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.ToInt();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("123", 123)]
		[TestCase(" 123", 123)]
		[TestCase(" 123 ", 123)]
		public void TakesCorrectString(string value, int outcome)
		{
			//Arrange
			var command = SupportedCommands.ToInt();

			//Act
			var result = (int)command.Run(GetContext(), value);

			//Assert
			result.Should().Be(outcome);
		}
	}
}