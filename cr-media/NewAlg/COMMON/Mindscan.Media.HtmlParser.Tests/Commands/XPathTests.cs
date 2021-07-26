using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class XPathTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
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
			var command = SupportedCommands.XPath();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div> <p>Waffle <br/> foo shikaka</p></div>", "<div> <p>Waffle <br /> foo shikaka</p></div>")]
		[TestCase("<div>&nbsp;<p>Waffle &copy;<br/> foo shikaka</p></div>", "<div> <p>Waffle ©<br /> foo shikaka</p></div>")]
		[TestCase("<div>&nbsp;<p>Waffle &copy;<br/> &lt;foo&gt; shikaka &amp;</p>&apos; &quot;</div>", "<div> <p>Waffle ©<br /> &lt;foo&gt; shikaka &amp;</p>' \"</div>")]
		public void TakesString(string html, string outcome)
		{
			//Arrange
			var command = SupportedCommands.XPath();

			//Act
			var result = (command.Run(GetContext(), html) as XDocument).ToString(SaveOptions.DisableFormatting);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}