using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class ConstTests : ParserTestsBase
	{
		[TestCase(null)]
		[TestCase(typeof(AttrTests))]
		[TestCase(666)]
		[TestCase("string")]
		[TestCase(new[] { 1, 2, 3 })]
		[TestCase("")]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Const();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		
		[TestCase("waffle")]
		[TestCase("tagiiil", "waffle", "shock")]
		public void TakesStrings(params string[] values)
		{
			//Arrange
			var command = SupportedCommands.Const(values);

			//Act
			var result = command.Run(GetContext(), null);

			//Assert
			if (values.Length == 1)
			{
				(result as string).Should().BeEquivalentTo(values[0]);
			}
			else if (values.Length > 0)
			{
				Assert.That(result as IEnumerable<string>, Is.EquivalentTo(values));
			}
		}

		[TestCase("", "", "")]
		[TestCase(null, null, null)]
		[TestCase(null, "", null)]
		[TestCase()]
		public void TakesEmpty(params string[] values)
		{
			//Arrange
			var command = SupportedCommands.Const(values);

			//Act
			var result = (object)command.Run(GetContext(), null);

			//Assert
			result.Should().BeNull();
		}
	}
}