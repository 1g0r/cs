using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class TextLinesTests : ParserTestsBase
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
			var command = SupportedCommands.TextLines(new[] { "p" });

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		private static object[] _cases1 =
		{
			new object[] {"", new string[] { }, null, null},
			new object[]
			{
				"<div><p>line one<br/>line two</p><p>line three</p><br/>line four</div>",
				new [] { "line one", "line two", "line three", "line four" },
				null, null
			},
			new object[]
			{
				"<div>Line one<p>line two</p>line three<p>Line four</p>line five</div>",
				new [] { "Line one", "line two", "line three", "Line four", "line five" },
				null, null
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div>",
				new [] { "line 1", "line 2", "line 3", "line 5" },
				new [] {".foo"}, null
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line <span>5</span></div>",
				new [] { "line 1", "line 2", "line 3", "line", "5" },
				new [] {".foo"},
				new [] {"span", "p", "br"}
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><p>line 2</p><br/>line 3<p class='foo'>line 4</p>line <span>5</span></div>",
				new [] { "line 1", "line 2", "line 3", "line", "5" },
				new [] {".foo"},
				new [] {"span", "p", "br"}
			},
			new object[]
			{
				"<div class=\"content\">" +
				"<p>line one<br/>line two</p><p>line three</p><br/>line four " +
				"<table><thead><tr><th>№</th><th>ФИО</th><th>Должность</th></tr></thead><tbody><tr><td><p>1</p></td><td><p>Канаев <br/>Кирилл Викторович</p></td><td><p>Глава управы района Бирюлево Восточное</p><p>Председатель комиссии</p></td></tr><tr><td><p>2</p></td><td>Карпинская Анна Павловна</td><td><p><font size=2>Заместитель главы управы Бирюлево Восточное по работе с населением</font><p>Заместитель председателя комиссии</p></td></tr></tbody></table>" +
				"Text after table</div>",
				new []
				{
					"line one",
					"line two",
					"line three",
					"line four",
					"№ ФИО Должность",
					"1 Канаев Кирилл Викторович Глава управы района Бирюлево Восточное Председатель комиссии",
					"2 Карпинская Анна Павловна Заместитель главы управы Бирюлево Восточное по работе с населением Заместитель председателя комиссии",
					"Text after table"
				},
				new [] {".foo"},
				new [] {"span", "p", "br"}
			},
			new object[]
			{
				"<div class=\"content\">" +
				"<span class='date'>01.01.2001</span>"+
				"<p> </p><br/>" +
				"<table><tbody><tr></tr><tr><td><p> </p></td></tr><tr><br/></tr></tbody></table>" +
				"</div>",
				new string[] {},
				new [] {".date"},
				new [] {"span", "p", "br"}
			}
		};
		[TestCaseSource(nameof(_cases1))]
		public void TakesIElement(string html, string[] outcome, string[] skipPatterns, string[] newLinePatterns)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns, newLinePatterns);
			var element = GetDocument(html).QuerySelector("div");

			//Act
			var result = (command.Run(GetContext(), element) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}

		private static object[] _cases2 =
		{
			new object[] {"", new string[] { }, null, null},
			new object[]
			{
				"<div><p>line one<br/>line two</p></div><div><p>line three</p><br/>line four</div>",
				new [] { "line one", "line two", "line three", "line four" },
				null, null
			},
			new object[]
			{
				"<div>Line one<p>line two</p></div><div>line three<p>Line four</p>line five</div>",
				new [] { "Line one", "line two", "line three", "Line four", "line five" },
				null,
				null
			},
			new object[]
			{
				"<div>line 1</div><div><p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div>",
				new [] { "line 1", "line 2", "line 3", "line 5" },
				new [] {".foo"}, null
			},
			new object[]
			{
				"<div>line 1<p>line 2</p></div><div><br/>line 3<p class='foo'>line 4</p>line <span class='foo'>5</span></div>",
				new [] { "line 1", "line 2", "line 3", "line" },
				new [] {".foo"},
				new [] {"p"}
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><p>line 2</p></div><div><br/>line 3<p class='foo'>line 4</p>line <span>5</span></div>",
				new [] { "line 1", "line 2", "line 3", "line", "5" },
				new [] {".foo"},
				new [] {"span", "p"}
			},
			new object[]
			{
				"<div class=\"content\">" +
				"<p>line one<br/>line two</p><p>line three</p><br/>line four </div>" +
				"<div><table><thead><tr><th>№</th><th>ФИО</th><th>Должность</th></tr></thead><tbody><tr><td><p>1</p></td><td><p>Канаев <br/>Кирилл Викторович</p></td><td><p>Глава управы района Бирюлево Восточное</p><p>Председатель комиссии</p></td></tr><tr><td><p>2</p></td><td>Карпинская Анна Павловна</td><td><p><font size=2>Заместитель главы управы Бирюлево Восточное по работе с населением</font><p>Заместитель председателя комиссии</p></td></tr></tbody></table>" +
				"Text after table</div>",
				new []
				{
					"line one",
					"line two",
					"line three",
					"line four",
					"№ ФИО Должность",
					"1 Канаев Кирилл Викторович Глава управы района Бирюлево Восточное Председатель комиссии",
					"2 Карпинская Анна Павловна Заместитель главы управы Бирюлево Восточное по работе с населением Заместитель председателя комиссии",
					"Text after table"
				},
				new [] {".foo"},
				new [] {"span", "p", "br"}
			},
			new object[]
			{
				"<div class=\"content\">" +
				"<span class='date'>01.01.2001</span>"+
				"<p> </p><br/>" +
				"<table><tbody><tr></tr><tr><td><p> </p></td></tr><tr><br/></tr></tbody></table>" +
				"</div>",
				new string[] {},
				new [] {".date"},
				new [] {"span", "p", "br"}
			}
		};
		[TestCaseSource(nameof(_cases2))]
		public void CollectionOfIElement(string html, string[] outcome, string[] skipPatterns, string[] newLinePatterns)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns, newLinePatterns);
			var elements = GetDocument(html).QuerySelectorAll("div").ToList();

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}

		private static object[] _cases3 =
		{
			new object[] {"<html></html>", new string[] { }, null, null},
			new object[]
			{
				"<html><div><p>line one<br/>line two</p><p>line three</p><br/>line four</div></html>",
				new [] { "line one", "line two", "line three", "line four" },
				null, null
			},
			new object[]
			{
				"<html><div>Line one<p>line two</p>line three<p>Line four</p>line five</div></html>",
				new [] { "Line one", "line two", "line three", "Line four", "line five" },
				null, null
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div>",
				new [] { "line 1", "line 2", "line 3", "line 5" },
				new [] { "*[@class='foo']" }, null
			},
			new object[]
			{
				"<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line <span>5</span></div>",
				new [] { "line 1", "line 2", "line 3", "line", "5" },
				new [] {"*[@class='foo']"},
				new [] {"span", "p", "br" }
			},
			new object[]
			{
				"<div><p>WAFFLE</p><p>WAFFLE</p><p>WAFFLE</p><p>WAFFLE</p></div>",
				new [] { "WAFFLE" },
				null,
				new [] {"span", "p", "br" }
			},
			new object[]
			{
				"<div class=\"content\">" +
				"<p>line one<br/>line two</p><p>line three</p><br/>line four " +
				"<table><thead><tr><th>№</th><th>ФИО</th><th>Должность</th></tr></thead><tbody><tr><td><p>1</p></td><td><p>Канаев <br/>Кирилл Викторович</p></td><td><p>Глава управы района Бирюлево Восточное</p><p>Председатель комиссии</p></td></tr><tr><td><p>2</p></td><td>Карпинская Анна Павловна</td><td><p><font size=2>Заместитель главы управы Бирюлево Восточное по работе с населением</font><p>Заместитель председателя комиссии</p></td></tr></tbody></table>" +
				"Text after table</div>",
				new []
				{
					"line one",
					"line two",
					"line three",
					"line four",
					"№ ФИО Должность",
					"1 Канаев Кирилл Викторович Глава управы района Бирюлево Восточное Председатель комиссии",
					"2 Карпинская Анна Павловна Заместитель главы управы Бирюлево Восточное по работе с населением Заместитель председателя комиссии",
					"Text after table"
				},
				new [] {"*[@class='foo']"},
				new [] {"span", "p", "br"}
			}
		};
		[TestCaseSource(nameof(_cases3))]
		public void TakesXElement(string html, string[] outcome, string[] skipPatterns, string[] newLinePatterns)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns, newLinePatterns);
			var element = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElement("//div");

			//Act
			var result = (command.Run(GetContext(), element) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}

		private static object[] _cases4 =
		{
			new object[] {"<html></html>", new string[] { }, null, null},
			new object[]
			{
				"<html><div><p>line one<br/>line two</p></div><div><p>line three</p><br/>line four</div></html>",
				new [] { "line one", "line two", "line three", "line four" },
				null,
				null
			},
			new object[]
			{
				"<html><div>Line one<p>line two</p></div><div>line three<p>Line four</p>line five</div></html>",
				new [] { "Line one", "line two", "line three", "Line four", "line five" },
				null,
				null
			},
			new object[]
			{
				"<html><div>line 1</div><div><p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div></html>",
				new [] { "line 1", "line 2", "line 3", "line 5" },
				new [] {"*[@class='foo']"}, null
			},
			new object[]
			{
				"<html><div>line 1<p>line 2</p></div><div><br/>line 3<p class='foo'>line 4</p>line <span>5</span></div></html>",
				new [] { "line 1", "line 2", "line 3", "line", "5" },
				new [] {"*[@class='foo']"},
				new [] {"p", "br", "span"}
			},
			new object[]
			{
				"<div><p>WAFFLE</p><p>WAFFLE</p><p>WAFFLE</p><p>WAFFLE</p></div>",
				new [] { "WAFFLE" },
				null,
				new [] {"span", "p", "br" },
			},
			new object[]
			{
				"<html><div class=\"content\">" +
				"<p>line one<br/>line two</p><p>line three</p><br/>line four </div>" +
				"<div><table><thead><tr><th>№</th><th>ФИО</th><th>Должность</th></tr></thead><tbody><tr><td><p>1</p></td><td><p>Канаев <br/>Кирилл Викторович</p></td><td><p>Глава управы района Бирюлево Восточное</p><p>Председатель комиссии</p></td></tr><tr><td><p>2</p></td><td>Карпинская Анна Павловна</td><td><p><font size=2>Заместитель главы управы Бирюлево Восточное по работе с населением</font><p>Заместитель председателя комиссии</p></td></tr></tbody></table>" +
				"Text after table</div></html>",
				new []
				{
					"line one",
					"line two",
					"line three",
					"line four",
					"№ ФИО Должность",
					"1 Канаев Кирилл Викторович Глава управы района Бирюлево Восточное Председатель комиссии",
					"2 Карпинская Анна Павловна Заместитель главы управы Бирюлево Восточное по работе с населением Заместитель председателя комиссии",
					"Text after table"
				},
				new [] {"*[@class='foo']"},
				new [] {"span", "p", "br"}
			}
		};
		[TestCaseSource(nameof(_cases4))]
		public void CollectionOfXElement(string html, string[] outcome, string[] skipPatterns, string[] newLinePatterns)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns, newLinePatterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElements("//div");

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}

		[TestCase("<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div>", "^\\w+\\s*")]
		public void TakesIElement_InvalidPattern(string html, string pattern)
		{
			//Arrange
			var command = SupportedCommands.TextLines(new[] { pattern });
			var element = GetDocument(html).QuerySelector("div");
			Exception ex = null;

			//Act
			try
			{
				(command.Run(GetContext(), element) as IEnumerable<string>).Count();
			}
			catch (InvalidOperationException e)
			{
				ex = e;
			}

			//Assert
			ex.Should().NotBeNull();
			ex.Should().BeAssignableTo(typeof(InvalidOperationException));
			ex.Message.Should().BeEquivalentTo($"Unable to match selector {pattern}");
		}

		[TestCase("<div>line 1<p>line 2</p><br/>line 3<p class='foo'>line 4</p>line 5</div>", "^\\w+\\s*")]
		public void TakesXElement_InvalidPattern(string html, string pattern)
		{
			//Arrange
			var command = SupportedCommands.TextLines(new[] { pattern });
			var element = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElement("//div");
			Exception ex = null;

			//Act
			try
			{
				(command.Run(GetContext(), element) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();
			}
			catch (InvalidOperationException e)
			{
				ex = e;
			}

			//Assert
			ex.Should().NotBeNull();
			ex.Should().BeAssignableTo(typeof(InvalidOperationException));
			ex.Message.Should().BeEquivalentTo($"Unable to match XPath {pattern}");
		}

		private static object[] _cases5 =
		{
			new object[] {"{data:[]}", new string[] { }, null},
			new object[]
			{
				"{data:['line one', 'line two', 'line three', 'line four']}",
				new [] { "line one", "line two", "line three", "line four" },
				null
			},
			new object[]
			{
				"{data:['line 1', 'line 2', 'line 3', 'line 5']}",
				new [] { "line 2", "line 3", "line 5" },
				new [] {"^Line 1"}
			}
		};
		[TestCaseSource(nameof(_cases5))]
		public void CollectionOfJToken(string json, string[] outcome, string[] skipPatterns)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns);
			var elements = JToken.Parse(json).SelectTokens("$..data.[*]");

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}

		[TestCase(
			"<div>line 1<p>line 2</p></div><div><br/>line 3<p class='foo'>line 4</p>line <span class='foo'>5</span></div>",
			new[] { ".foo" },
			new[] { "line 1", "line 2", "line 3", "line" }
		)]
		public void ShouldSkipIElements(string html, string[] skipPatterns, string[] outcome)
		{
			//Arrange
			var command = SupportedCommands.TextLines(skipPatterns);
			var elements = GetDocument(html).QuerySelectorAll("div").ToList();

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToList();

			//Assert
			result.Should().NotBeNull();
			result.Count.Should().Be(outcome.Length);
			if (outcome.Length > 0)
			{
				for (var i = 0; i < outcome.Length; ++i)
				{
					result[i].Should().BeEquivalentTo(outcome[i]);
				}
			}
		}
	}
}