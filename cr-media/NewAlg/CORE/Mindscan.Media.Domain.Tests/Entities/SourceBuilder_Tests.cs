using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;
using Mindscan.Media.Domain.Exceptions;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class SourceBuilder_Tests
	{
		[Test]
		public void Throw_If_Call_Twice()
		{
			//Arrange
			var builder = Source.GetBuilder()
				.Url(new Uri("http://source.uri"))
				.Name("name");
			var source = builder.Build();

			//Act
			var error = Assert.Throws<InvalidOperationException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Build method can be called only ones.");
		}

		[Test]
		public void ShouldNot_BuildWithEmptyUrl()
		{
			//Arrange
			var builder = Source.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Name("sdfsd").Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Contain("Url");
		}

		[Test]
		public void ShouldNot_BuildWithEmptyName()
		{
			//Arrange
			var builder = Source.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Url(new Uri("http://test.com")).Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Contain("Name");
		}

		[Test]
		public void Should_SetDefaultTypeToSmi()
		{
			//Arrange
			var builder = Source.GetBuilder();
			var url = new Uri("http://test.com");

			//Act
			var source = builder
				.Url(url)
				.Name("Test")
				.Build();

			//Assert
			source.Should().NotBeNull();
			source.Type.Should().Be(SourceType.Smi);
			source.Name.Should().Be("Test");
			source.Url.Value.Should().BeEquivalentTo(url);
		}

		[Test]
		public void ShouldNot_SetEmptyUrl()
		{
			//Arrange
			var builder = Source.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Url(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Contain("Url");
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("    ")]
		public void ShouldNot_SetEmptyName(string name)
		{
			//Arrange
			var builder = Source.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Name(name));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Contain("Name");
		}
	}
}
