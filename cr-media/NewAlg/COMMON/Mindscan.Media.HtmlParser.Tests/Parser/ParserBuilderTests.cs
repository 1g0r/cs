using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core;
using Mindscan.Media.HtmlParser.Core.Parser;
using Mindscan.Media.HtmlParser.Core.Schema;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Parser
{
	[Category("UnitTests")]
	public class ParserBuilderTests: ParserTestsBase
	{
		[Test]
		public void PipelineExpression()
		{
			//Arrange
			var names = SupportedNames.GetCommandNames().ToList();
			var commandsJson = GetAllCommandsJson();

			//Act
			var commands = ParserBuilderHelper.ParsePipelineCode(commandsJson).ToArray();
			var json = ParserBuilderHelper.BuildPipelineCode(commands);

			//Assert
			json.Should().NotBeNullOrEmpty();
			json.Should().BeEquivalentTo(commandsJson);

			foreach (var name in names)
			{
				var cmd = commands.Cast<CustomJsonObject>().FirstOrDefault(c => c.Name == name);
				cmd.Should().NotBeNull();
			}
		}

		[Test]
		public void TestExpressionsCode()
		{
			//Arrange
			var templateParser = GetTemplateParser();
			var templateParserJson = templateParser.ToJson(false, true);

			//Act
			var resultParser = ParserBuilder.CreateParser(templateParserJson);
			var jsonWithCode1 = templateParser.ToJson(true, true);
			var jsonWithCode2 = resultParser.ToJson(true, true);
			var resultParserJson = ParserBuilder.CreateParser(jsonWithCode2).ToJson(false, true);

			//Assert
			jsonWithCode1.Should().BeEquivalentTo(jsonWithCode2);
			resultParserJson.Should().BeEquivalentTo(templateParserJson);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("     ")]
		[TestCase("Какая то строка")]
		public void Should_AcceptEmptyContent(string content)
		{
			//Arrange
			var parser = GetTemplateParser();
			var pageUrl = new Uri("http://texts.com");

			//Act
			var result = parser.ParsePage(content, pageUrl, true, false);

			//Assert
			result.Should().Be(string.Empty);
		}

		private static HtmlPageParser GetTemplateParser()
		{
			return new HtmlPageParser
			{
				Encoding = "utf-7",
				Schema = new ComplexValue
				{
					Expression = ExpressionBuilder.For(
						ExpressionBuilder.Or(
							ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
							ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
							ExpressionBuilder.Or(
								ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
								ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
								ExpressionBuilder.And(
									ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
									ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
									ExpressionBuilder.Or(
										ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
										ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
										ExpressionBuilder.And(
											ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
											ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray())
										)
									)
								)
							)
						),
						ExpressionBuilder.And(
							ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
							ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
							ExpressionBuilder.Or(
								ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
								ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
								ExpressionBuilder.And(
									ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
									ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
									ExpressionBuilder.Or(
										ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
										ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
										ExpressionBuilder.And(
											ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray()),
											ExpressionBuilder.Pipeline(ParserBuilderHelper.ParsePipelineCode(GetAllCommandsJson()).ToArray())
										)
									)
								)
							)
						)
					)	
				}
			};
		}

		private static string GetAllCommandsJson()
		{
			var names = SupportedNames.GetCommandNames().ToList();
			var codeBuilder = new StringBuilder();
			bool first = true;
			foreach (var name in names)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					codeBuilder.Append(".");
				}
				codeBuilder.Append(name);
				codeBuilder.Append("()");
			}
			return codeBuilder.ToString();
		}
	}
}
