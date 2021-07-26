using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class ReplaceTests : ParserTestsBase
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
			var command = SupportedCommands.Replace("", "");

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("//sdflkj/sdflkj/sdflkj", null, null, null)]
		[TestCase("//sdflkj/sdflkj/sdflkj", "", null, null)]
		[TestCase("//sdflkj/sdflkj/sdflkj", "//", null, "sdflkj/sdflkj/sdflkj")]
		[TestCase("//sdflkj/sdflkj/sdflkj", "//", "", "sdflkj/sdflkj/sdflkj")]
		[TestCase("//sdflkj/sdflkj/sdflkj", "//", "http://", "http://sdflkj/sdflkj/sdflkj")]
		[TestCase("Waffle foo shikaka", "foo", "    ", "Waffle shikaka")]
		[TestCase("Waffle foo shikaka", "Waffle foo shikaka", "", null)]
		public void TakesString(string value, string oldValue, string newValue, string outcome)
		{
			//Arrange
			var command = SupportedCommands.Replace(oldValue, newValue);

			//Act
			var result = (string)command.Run(GetContext(), value);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}

		[TestCase(new[] { "//sdflkj/sdflkj/sdflkj", "//sdflkj/sdflkj/sdflkj" }, null, null, null)]
		[TestCase(new[] { "//sdflkj/sdflkj/sdflkj", "//sdflkj/sdflkj/sdflkj" }, "", null, null)]
		[TestCase(new[] { "//sdflkj/sdflkj/sdflkj", "//sdflkj/sdflkj/sdflkj" }, "//", null, new[] { "sdflkj/sdflkj/sdflkj", "sdflkj/sdflkj/sdflkj" })]
		[TestCase(new[] { "//sdflkj/sdflkj/sdflkj", "//sdflkj" }, "//", "http://", new[] { "http://sdflkj/sdflkj/sdflkj", "http://sdflkj" })]
		[TestCase(new[] { "Waffle foo shikaka" }, "foo", "     ", new[] { "Waffle shikaka" })]
		[TestCase(new[] { "Waffle foo shikaka" }, "Waffle foo shikaka", "", null)]
		public void TakesCollectionOfStrings(string[] values, string oldValue, string newValue, string[] outcome)
		{
			//Arrange
			var command = SupportedCommands.Replace(oldValue, newValue);

			//Act
			var result = (command.Run(GetContext(), values) as IEnumerable<string>)?.ToArray();

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}