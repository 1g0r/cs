using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Moq;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	public class TriggersTestsBase
	{
		protected TriggerBuilder CreateBuilder(DateTime updatedAt = default(DateTime))
		{
			if (updatedAt == default(DateTime))
				updatedAt = DateTime.UtcNow;
			return Trigger.GetBuilder()
				.Id(1)
				.Enabled(true)
				.RoutingKey("RoutingKey")
				.UpdatedAtUtc(updatedAt)
				.CreatedAtUtc(DateTime.UtcNow)
				.RepeatInterval(new TimeSpan(1, 30, 0));
		}

		protected static IFeedsRepository GetFeedRepository(bool returnNull = false)
		{
			Feed feed = returnNull ? null : Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.uri"))
				.Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(feed);

			return repo.Object;
		}

		protected static Mock<ITriggersRepository> GetTriggersRepo(bool exist = false, bool returnNull = false)
		{
			var repo = new Mock<ITriggersRepository>();
			repo.Setup(x => x.Exists(It.IsAny<Trigger>()))
				.Returns(exist);
			repo.Setup(x => x.Add(It.IsAny<Trigger>()))
				.Returns((Trigger t) => returnNull ? null : t);

			return repo;
		}

		protected static ISourcesRepository GetSourcesRepo()
		{
			return new Mock<ISourcesRepository>().Object;
		}
	}
}