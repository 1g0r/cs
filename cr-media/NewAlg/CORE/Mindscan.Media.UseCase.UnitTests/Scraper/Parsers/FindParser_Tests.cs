using System;
using System.Collections.Generic;
using FluentAssertions;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scraper.Parsers;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scraper.Parsers
{
	[TestFixture]
	public class FindParser_Tests : ParserTestsBase
	{
		[Test]
		public void Throw_If_Not_Found_By_Id()
		{
			//Arrange
			var repo = new Mock<IParsersRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns((Parser) null);
			var uk = new FindParser(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Find(1));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Parser with Id=1 not found.");
		}

		[Test]
		public void Success_By_Id()
		{
			//Arrange
			var parser = CreateParserBuilder().Build();
			var repo = new Mock<IParsersRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(parser);
			var uk = new FindParser(repo.Object);
			

			//Act
			var result = uk.Find(1);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(parser);
		}

		[Test]
		public void Throw_If_PageUrl_Null()
		{
			//Arrange
			var uk = new FindParser(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Find(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: pageUrl");
		}

		[Test]
		public void Throw_If_Repository__Returned_Empty()
		{
			//Arrange
			var repo = new Mock<IParsersRepository>();
			repo.Setup(x => x.Find(It.IsAny<NormalizedUrl>(), It.IsAny<string>()))
				.Returns((IEnumerable<Tuple<Parser, Source>>)null);
			var uk = new FindParser(repo.Object);
			var pageUrl = NormalizedUrl.Build(new Uri("http://page.url"));

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Find(pageUrl));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Parser for page with URL 'http://page.url/' not found.");
		}

		[Test]
		public void Success_Longest_Path_Match()
		{
			//Arrange
			var source = Source.GetBuilder().Id(1).Url(new Uri("http://source.uri")).Name("Source Name").Build();
			
			var successPath = "/path/number/1/or";
			var relations = new List<Tuple<Parser, Source>>
			{
				new Tuple<Parser, Source>(CreateParserBuilder().Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder("/path/").Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder("/path/number").Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder("/path/number/1").Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder("/path/number/2").Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder(successPath).Build(), source),
			};
			var repo = new Mock<IParsersRepository>();
			repo.Setup(x => x.Find(It.IsAny<NormalizedUrl>(), It.IsAny<string>())).Returns(relations);
			var uk = new FindParser(repo.Object);
			var pageUrl = NormalizedUrl.Build(new Uri($"http://host.uri{successPath}/some/other"));

			//Act
			var result = uk.Find(pageUrl);

			//Assert
			result.Should().NotBeNull();
			result.Item1.Should().NotBeNull();
			result.Item1.Path.Should().Be(successPath);
			result.Item2.Should().BeEquivalentTo(source);
		}

		[Test]
		public void Success_Longest_Host_Match()
		{
			//Arrange
			var source = Source.GetBuilder().Id(1).Url(new Uri("http://source.uri")).Name("Source Name").Build();

			var successHost = "aaaa.info.vesti.ru";
			var relations = new List<Tuple<Parser, Source>>
			{
				new Tuple<Parser, Source>(CreateParserBuilder().Host(new Uri("http://vesti.ru")).Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder().Host(new Uri($"http://info.vesti.ru")).Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder().Host(new Uri($"http://info.vesti.ru")).Tag("board").Build(), source),
				new Tuple<Parser, Source>(CreateParserBuilder().Host(new Uri($"http://{successHost}")).Build(), source),
			};
			var repo = new Mock<IParsersRepository>();
			repo.Setup(x => x.Find(It.IsAny<NormalizedUrl>(), It.IsAny<string>())).Returns(relations);
			var uk = new FindParser(repo.Object);
			var pageUrl = NormalizedUrl.Build(new Uri($"http://{successHost}/some/other"));

			//Act
			var result = uk.Find(pageUrl);

			//Assert
			result.Should().NotBeNull();
			result.Item1.Should().NotBeNull();
			result.Item1.Host.Host.Should().Be(successHost);
			result.Item2.Should().BeEquivalentTo(source);
		}
	}
}
