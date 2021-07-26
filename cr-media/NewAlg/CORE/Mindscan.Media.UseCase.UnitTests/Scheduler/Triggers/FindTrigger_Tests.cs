using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Scheduler.Triggers;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	[TestFixture]
	public class FindTrigger_Tests : TriggersTestsBase
	{
		[Test]
		public void Throw_If_Filter_Null()
		{
			//Arrange
			var uk = new FindTrigger(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Find(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: filter");
		}

		[Test]
		public void Return_Empty_Enumerable()
		{
			//Arrange
			var repo = GetTriggersRepo();
			repo.Setup(x => x.FindTriggersToFire()).Returns((IEnumerable<Trigger>)null);
			repo.Setup(x => x.Find(It.IsAny<TriggerFilter>())).Returns((IEnumerable<Tuple<Trigger, Feed, Source>>)null);
			var uk = new FindTrigger(repo.Object);
			var filter = new TriggerFilter();

			//Act
			var result1 = uk.FindTriggersToFire();
			var result2 = uk.Find(filter);

			//Assert
			result1.Should().NotBeNull();
			result1.Count().Should().Be(0);
			result2.Should().NotBeNull();
			result2.Count().Should().Be(0);
		}

		[Test]
		public void Success_Triggers_ToFire()
		{
			//Arrange
			var triggers = new List<Trigger>
			{
				Trigger.GetBuilder().Id(1).RoutingKey("Smi").RepeatInterval(new TimeSpan(0, 30, 0)).Build(),
				Trigger.GetBuilder().Id(2).RoutingKey("Smi").RepeatInterval(new TimeSpan(0, 30, 0)).Build()
			};
			var repo = GetTriggersRepo();
			repo.Setup(x => x.FindTriggersToFire()).Returns(triggers);
			var uk = new FindTrigger(repo.Object);
			
			//Act
			var result = uk.FindTriggersToFire();
			
			//Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(triggers.Count);
			result.Should().BeEquivalentTo(triggers);
		}

		[Test]
		public void Success_Find()
		{
			//Arrange
			var relation = new List<Tuple<Trigger, Feed, Source>>
			{
				new Tuple<Trigger, Feed, Source>(
					Trigger.GetBuilder().Id(1).RoutingKey("Smi").RepeatInterval(new TimeSpan(0, 30, 0)).Build(),
					Feed.GetBuilder().Id(1).OriginalUrl(new Uri("http://feed.uri")).Build(),
					Source.GetBuilder().Id(1).Url(new Uri("http://source.uri")).Name("source").Build()
				),
				new Tuple<Trigger, Feed, Source>(
					Trigger.GetBuilder().Id(2).RoutingKey("Smi").RepeatInterval(new TimeSpan(0, 30, 0)).Build(),
					Feed.GetBuilder().Id(2).OriginalUrl(new Uri("http://feed.uri")).Build(),
					Source.GetBuilder().Id(2).Url(new Uri("http://source.uri")).Name("source").Build()
				),
			};
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<TriggerFilter>())).Returns(relation);
			var uk = new FindTrigger(repo.Object);

			//Act
			var result = uk.Find(new TriggerFilter());

			//Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(2);
			result.Should().BeEquivalentTo(relation);
		}
	}
}