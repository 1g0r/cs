using System;
using System.Configuration;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.Utils.UnitTests.Config
{
	[Category("UnitTests")]
	[TestFixture]
	public class ConfigBaseTests
	{
		[Test]
		public void BuildSettings_ShouldBeCalled()
		{
			//Arrange
			var xml = new XElement("testSection");

			//Act
			var section = new ConfigBaseStub(xml);

			//Assert
			section.BuildSettingsWasCalled.Should().BeTrue();
		}

		[Test]
		public void Element_EmptyDefaultValue_HasValue()
		{
			//Arrange
			string value = "Waffle";
			var xml = new XElement("testSection", new XElement("Name", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetElementValue("Name");

			//Assert
			result.Should().BeEquivalentTo(value);

		}

		[Test]
		public void Element_EmptyDefaultValue_NoValue()
		{
			//Arrange
			var xml = new XElement("testSection");
			var section = new ConfigBaseStub(xml);

			//Act
			var ex = Assert.Throws<ConfigurationErrorsException>(() =>
			{
				section.GetElementValue("Name");
			});

			//Assert

			ex.Message.Should().BeEquivalentTo("Mandatory setting 'Name' is empty. Please fill it in and start all over again.");
		}

		[Test]
		public void Element_NotEmptyDefaultValue_String()
		{
			//Arrange
			string defaultValue = "Waffle";
			string value = "Foo";
			var xml = new XElement("testSection", new XElement("Name", "Foo"));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetElementValue("Name", defaultValue);
			var result2 = section.GetElementValue("Key", defaultValue);

			//Assert
			result.Should().BeEquivalentTo(value);
			result2.Should().BeEquivalentTo(defaultValue);
		}

		[Test]
		public void Element_NotEmptyDefaultValue_Int()
		{
			//Arrange 
			int value = 100;
			int defaultValue = 10;
			var xml = new XElement("testSection", new XElement("Age", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetElementInt("Age", defaultValue);
			var result2 = section.GetElementInt("TestKey", defaultValue);

			//Assert
			result.Should().Be(value);
			result2.Should().Be(defaultValue);
		}

		[Test]
		public void Element_WrongStringValue()
		{
			//Arrange
			int defaultValue = 100;
			var xml = new XElement("testSection", new XElement("Age", "one year"));
			var section = new ConfigBaseStub(xml);

			//Act
			var ex = Assert.Throws<InvalidCastException>(() => section.GetElementInt("Age", defaultValue));

			//Assert

			ex.Message.Should().BeEquivalentTo("Unable to cast 'one year' value.");
		}

		[Test]
		public void Element_EmptyCastFunction()
		{
			//Arrange
			var value = TimeSpan.FromMinutes(666);
			var xml = new XElement("testSection", new XElement("Inteval", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var ex = Assert.Throws<InvalidCastException>(() => section.GetElementTimeSpan_EmptyCast("Interval", value));

			//Assert
			ex.Message.Should().BeEquivalentTo("Cast function must not be empty.");
		}

		[Test]
		public void Element_NotEmptyDefaultValue_TimeSpan()
		{
			//Arrange 
			TimeSpan value = TimeSpan.FromMinutes(666);
			TimeSpan defaultValue = TimeSpan.FromMinutes(10); 
			var xml = new XElement("testSection", new XElement("Interval", value.ToString()));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetElementTimeSpan("Interval", defaultValue);
			var result2 = section.GetElementTimeSpan("TestKey", defaultValue);

			//Assert
			result.Should().Be(value);
			result2.Should().Be(defaultValue);
		}

		[Test]
		public void Attribute_EmptyDefaultValue_HasValue()
		{
			//Arrange
			string value = "Waffle";
			var xml = new XElement("testSection", new XAttribute("Name", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetAttributeValue("Name");

			//Assert
			result.Should().BeEquivalentTo(value);

		}

		[Test]
		public void Attribute_EmptyDefaultValue_NoValue()
		{
			//Arrange
			var section = new ConfigBaseStub(new XElement("testSection"));

			//Act
			var ex = Assert.Throws<ConfigurationErrorsException>(() =>
			{
				section.GetAttributeValue("Name");
			});

			//Assert
			ex.Message.Should().BeEquivalentTo("Mandatory setting 'Name' is empty. Please fill it in and start all over again.");
		}

		[Test]
		public void Attribute_NotEmptyDefaultValue_String()
		{
			//Arrange
			string defaultValue = "Waffle";
			string value = "Foo";
			var xml = new XElement("testSection", new XAttribute("Name", "Foo"));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetAttributeValue("Name", defaultValue);
			var result2 = section.GetAttributeValue("Key", defaultValue);

			//Assert
			result.Should().BeEquivalentTo(value);
			result2.Should().BeEquivalentTo(defaultValue);
		}

		[Test]
		public void Attribute_NotEmptyDefaultValue_Int()
		{
			//Arrange 
			int value = 100;
			int defaultValue = 10;
			var xml = new XElement("testSection", new XAttribute("Age", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetAttributeInt("Age", defaultValue);
			var result2 = section.GetAttributeInt("TestKey", defaultValue);

			//Assert
			result.Should().Be(value);
			result2.Should().Be(defaultValue);
		}

		[Test]
		public void Attribute_WrongStringValue()
		{
			//Arrange
			int defaultValue = 100;
			var xml = new XElement("testSection", new XAttribute("Age", "one year"));
			var section = new ConfigBaseStub(xml);

			//Act
			var ex = Assert.Throws<InvalidCastException>(() =>
			{
				section.GetAttributeInt("Age", defaultValue);
			});

			//Assert
			ex.Message.Should().BeEquivalentTo("Unable to cast 'one year' value.");
		}

		[Test]
		public void Attribute_EmptyCastFunction()
		{
			//Arrange
			var value = TimeSpan.FromMinutes(666);
			var xml = new XElement("testSection", new XAttribute("Inteval", value));
			var section = new ConfigBaseStub(xml);

			//Act
			var ex = Assert.Throws<InvalidCastException>(() => section.GetAttributeTimeSpan_EmptyCast("Interval", value));

			//Assert

			ex.Message.Should().BeEquivalentTo("Cast function must not be empty.");
		}

		[Test]
		public void Attribute_NotEmptyDefaultValue_TimeSpan()
		{
			//Arrange 
			TimeSpan value = TimeSpan.FromMinutes(666);
			TimeSpan defaultValue = TimeSpan.FromMinutes(10);
			var xml = new XElement("testSection", new XAttribute("Interval", value.ToString()));
			var section = new ConfigBaseStub(xml);

			//Act
			var result = section.GetAttributeTimeSpan("Interval", defaultValue);
			var result2 = section.GetAttributeTimeSpan("TestKey", defaultValue);

			//Assert
			result.Should().Be(value);
			result2.Should().Be(defaultValue);
		}

		[Test]
		public void SectionName_NotFound()
		{
			//Arrange
			var sectionName = "Waffle";

			//Act
			var ex = Assert.Throws<ConfigurationErrorsException>(() => new ConfigBaseStub(sectionName));

			//Assert
			ex.Message.Should().BeEquivalentTo($"Unable to find section '{sectionName}' in configuration file.");
		}
	}
}
