using System;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class JsonTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			null,
			"",
			typeof(AttrTests),
			new[] { 1, 2, 3 }
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Json();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("{images:[], text:'waffle'}", "{\"images\":[],\"text\":\"waffle\"}")]
		[TestCase("{images:[], text:'waffle}", null, true)]
		[TestCase("", null)]
		[TestCase("waffle", null, true)]
		public void TakesString(string json, string outcome, bool fault = false)
		{
			//Arrange
			var command = SupportedCommands.Json();
			JsonReaderException exception = null;
			string result = null;

			//Act
			try
			{
				result = (command.Run(GetContext(), json) as JToken)?.ToString(Formatting.None);
			}
			catch (JsonReaderException e)
			{
				exception = e;
			}


			//Assert
			if (fault)
			{
				exception.Should().NotBeNull();
			}
			else
			{
				result.Should().BeEquivalentTo(outcome);
			}
		}
	}
}