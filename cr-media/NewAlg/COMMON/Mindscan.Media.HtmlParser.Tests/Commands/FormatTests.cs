using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class FormatTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests)
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Format("sdfkj");

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}


		[TestCase("{0}.{1}", "waffle.foo", "waffle", "foo")]
		[TestCase("{0}.{1}", "waffle.foo", "waffle", "foo", "one")]
		[TestCase("{0}.{1}", "{0}.{1}", "waffle")]
		[TestCase("{0}.{1}", "{0}.{1}")]
		public void TakesPatternOnMultipleValue(string pattern, string outcome, params string[] values)
		{
			//Arrange
			var command = SupportedCommands.Format(pattern);

			//Act
			var result = (string)command.Run(GetContext(), values);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}

		[TestCase("{0}.foo", "waffle.foo", "waffle")]
		[TestCase("{0}.{1}", "{0}.{1}", "waffle")]
		[TestCase("{0}.{1}", "{0}.{1}", "")]
		public void TakesPatternOnSingleValue(string pattern, string outcome, string value)
		{
			//Arrange
			var command = SupportedCommands.Format(pattern);

			//Act
			var result = (string)command.Run(GetContext(), value);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}