using System;
using FluentAssertions;
using Mindscan.Media.Domain.Const;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class ParserBuilder_Tests
	{
		[Test]
		public void Throw_If_Call_Twice()
		{
			//Arrange
			var builder = Parser.GetBuilder()
				.Host(new Uri("http://host.com"));
			var parser = builder.SourceId(1).Build();

			//Act
			var error = Assert.Throws<InvalidOperationException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Build method can be called only ones.");
		}

		[Test]
		public void Requires_SourceId()
		{
			//Arrange
			var builder = Parser.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: SourceId");
		}

		[Test]
		public void Requires_Host()
		{
			//Arrange
			var builder = Parser.GetBuilder().SourceId(1);

			//Act
			var error1 = Assert.Throws<RequiredFieldException>(() => builder.Host(null));
			var error2 = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error1.Should().NotBeNull();
			error1.Message.Should().Be("Value cannot be null.\r\nParameter name: Host");

			error2.Should().NotBeNull();
			error2.Message.Should().Be("Value cannot be null.\r\nParameter name: Host");
		}

		[Test]
		public void Default_Encoding()
		{
			//Arrange
			var host = new Uri("http://host.ru");

			//Act
			var p1 = Parser.GetBuilder().SourceId(1).Host(host).Encoding(null).Build();
			var p2 = Parser.GetBuilder().SourceId(1).Host(host).Encoding("").Build();
			var p3 = Parser.GetBuilder().SourceId(1).Host(host).Encoding(" ").Build();
			var p4 = Parser.GetBuilder().SourceId(1).Host(host).Build();

			//Assert
			p1.Encoding.Should().Be(GlobalDefaults.Encoding);
			p2.Encoding.Should().Be(GlobalDefaults.Encoding);
			p3.Encoding.Should().Be(GlobalDefaults.Encoding);
			p4.Encoding.Should().Be(GlobalDefaults.Encoding);
		}

		[TestCase("/DDD/sdf", "/ddd/sdf")]
		[TestCase("  /DDD/sdf  ", "/ddd/sdf")]
		[TestCase("    ", null)]
		[TestCase(null, null)]
		public void Lower_Path(string value, string outcome)
		{
			//Arrange
			var builder = Parser.GetBuilder()
				.SourceId(1)
				.Host(new Uri("http://host.com"))
				.Path(value);

			//Act
			var parser = builder.Build();

			//Assert
			parser.Should().NotBeNull();
			parser.Path.Should().Be(outcome);
		}

		[TestCase(null)]
		[TestCase("   ")]
		[TestCase("")]
		public void Default_Tag(string value)
		{
			//Arrange
			var builder = Parser.GetBuilder()
				.SourceId(1)
				.Host(new Uri("http://host.com/"))
				.Tag(value);

			//Act 
			var parser = builder.Build();

			//Assert
			parser.Should().NotBeNull();
			parser.Tag.Should().BeNull();
		}
	}
}