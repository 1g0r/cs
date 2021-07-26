using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class UnescapeTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			DateTimeOffset.Now,
			null,
			new[] { 1, 2, 3 },
			typeof(AttrTests)
		};

		[TestCaseSource(nameof(_cases))]
		public void NotSupportedValueType(object value)
		{
			//Arrange
			var command = SupportedCommands.Unescape();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase(@"\u0432\u044b\u0441\u0442\u0430\u0432\u043a\u0430")]
		public void TakesString(string unicode)
		{
			//Arrange
			var command = SupportedCommands.Unescape();

			//Act
			var result = (string)command.Run(GetContext(), unicode);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo("выставка");
		}


	}
}