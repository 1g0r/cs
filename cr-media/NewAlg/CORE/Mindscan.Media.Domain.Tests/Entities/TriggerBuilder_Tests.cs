using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class TriggerBuilder_Tests
	{
		[Test]
		public void Throw_If_Call_Twice()
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("name")
				.RepeatInterval(new TimeSpan(0,10,14));
			var trigger = builder.Build();

			//Act
			var error = Assert.Throws<InvalidOperationException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Build method can be called only ones.");
		}

		[Test]
		public void FireTime_ShouldBe_Utc()
		{
			//Arrange
			var builder = Trigger.GetBuilder();
			var fireTime = DateTime.Now;

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.FireTimeUtc(fireTime));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: FireTimeUtc");
		}

		[TestCase("")]
		[TestCase("    ")]
		[TestCase(null)]
		[TestCase(null, false)]
		public void RoutingKey_Required(string value, bool set = true)
		{
			//Arrange
			var builder = Trigger.GetBuilder();
			if (set)
			{

			}

			//Act
			var error = Assert.Throws<RequiredFieldException>(() =>
			{
				if (set)
				{
					builder.RoutingKey(value);
				}
				else
				{
					builder.Build();
				}
			});

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: RoutingKey");
		}

		[TestCase("   foo", "foo")]
		[TestCase("foo   ", "foo")]
		[TestCase("   foo  ", "foo")]
		[TestCase("  waffle   foo  ", "waffle   foo")]
		public void RoutingKey_ShouldBe_Trimmed(string value, string outcome)
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RepeatInterval(DateTime.Now.TimeOfDay);

			//Act
			var trigger = builder
				.RoutingKey(value)
				.Build();

			//Assert
			trigger.Should().NotBeNull();
			trigger.RoutingKey.Should().Be(outcome);
		}

		[Test]
		public void RepeatInterval_Required()
		{
			//Arrange
			var builder = Trigger
				.GetBuilder()
				.RoutingKey("fff");

			//Act
			var error = Assert.Throws<RequiredFieldException>(() =>  builder.Build());


			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: RepeatInterval");
		}

		private static object[] _intervals = {
			TimeSpan.Zero,
			TimeSpan.FromMinutes(4),
			TimeSpan.FromMinutes(5)
		};
		[TestCaseSource(nameof(_intervals))]
		public void RepeatInterval_Range_Valid(TimeSpan value)
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("foo");

			//Act
			var error = Assert.Throws<ArgumentOutOfRangeException>(() => builder.RepeatInterval(value));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Specified argument was out of the range of valid values.\r\nParameter name: RepeatInterval");
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("    ")]
		public void VirtualHost_ShouldBe_Default(string value)
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("Foo")
				.RepeatInterval(DateTime.Now.TimeOfDay)
				.VirtualHost(value);

			//Act
			var trigger = builder.Build();

			//Assert
			trigger.Should().NotBeNull();
			trigger.VirtualHost.Should().Be("Scheduler");
		}

		[TestCase("f", "f")]
		[TestCase("fuu", "fuu")]
		[TestCase("  d  ", "d")]
		public void VirtualHost_ShouldNotBe_Default(string value, string outcome)
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("Foo")
				.RepeatInterval(DateTime.Now.TimeOfDay)
				.VirtualHost(value);

			//Act
			var trigger = builder.Build();

			//Assert
			trigger.Should().NotBeNull();
			trigger.VirtualHost.Should().Be(outcome);
		}

		[Test]
		public void StartAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("Foo")
				.RepeatInterval(DateTime.Now.TimeOfDay);

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.StartAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: StartAtUtc");
		}

		[Test]
		public void CreatedAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("Foo")
				.RepeatInterval(DateTime.Now.TimeOfDay);

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.CreatedAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: CreatedAtUtc");
		}

		[Test]
		public void UpdatedAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var builder = Trigger.GetBuilder()
				.RoutingKey("Foo")
				.RepeatInterval(DateTime.Now.TimeOfDay);

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.UpdatedAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: UpdatedAtUtc");
		}
	}
}