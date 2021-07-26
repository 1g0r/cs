using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class DecodeTests : ParserTestsBase
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
			var command = SupportedCommands.Decode();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase(@"&#034;type&#034;:&#034;video&#034;,&#034;_id&#034;:&#034;a9b1ab12-a495-11e7-b573-8ec86cdfe1ed&#034;")]
		public void TakesString(string unicode)
		{
			//Arrange
			var command = SupportedCommands.Decode();

			//Act
			var result = (string)command.Run(GetContext(), unicode);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo("\"type\":\"video\",\"_id\":\"a9b1ab12-a495-11e7-b573-8ec86cdfe1ed\"");
		}
	}
}