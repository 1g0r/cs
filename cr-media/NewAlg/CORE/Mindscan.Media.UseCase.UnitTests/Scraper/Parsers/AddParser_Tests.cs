using System;
using FluentAssertions;
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
	public class AddParser_Tests : ParserTestsBase
	{
		[Test]
		public void Throw_If_Parser_Null()
		{
			//Arrange
			var uk = new AddParser(null, null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Add(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: parser");
		}

		[Test]
		public void Throw_If_Source_Not_Found()
		{
			//Arrange
			var sourceRepo = new Mock<ISourcesRepository>();
			sourceRepo.Setup(x => x.Find(It.IsAny<long>())).Returns((Source) null);
			var uk = new AddParser(sourceRepo.Object, null);
			var parser = CreateParserBuilder().Build();

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Add(parser));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Source with id=1 not found.");
		}

		[Test]
		public void Throw_If_Repository_returned_Null()
		{
			//Arrange
			var source = Source.GetBuilder().Id(1).Url(new Uri("http://source.url")).Name("Source Name").Build();
			var sourceRepo = new Mock<ISourcesRepository>();
			sourceRepo.Setup(x => x.Find(It.IsAny<long>())).Returns(source);
			var parserRepo = new Mock<IParsersRepository>();
			parserRepo.Setup(x => x.Add(It.IsAny<Parser>())).Returns((Parser) null);
			var uk = new AddParser(sourceRepo.Object, parserRepo.Object);
			var parser = CreateParserBuilder().Build();

			//Act
			var error = Assert.Throws<EntityFailToAddException>(() => uk.Add(parser));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Unable to add Parser.");
		}

		[Test]
		public void Success_Return_New_Parser()
		{
			//Arrange
			var source = Source.GetBuilder().Id(1).Url(new Uri("http://source.url")).Name("Source Name").Build();
			var sourceRepo = new Mock<ISourcesRepository>();
			sourceRepo.Setup(x => x.Find(It.IsAny<long>())).Returns(source);

			var parser = CreateParserBuilder().Build();
			var parserRepo = new Mock<IParsersRepository>();
			parserRepo.Setup(x => x.Add(It.IsAny<Parser>())).Returns(parser);

			var uk = new AddParser(sourceRepo.Object, parserRepo.Object);
			

			//Act
			var result = uk.Add(parser);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(parser);
		}
	}
}
