using System;
using Mindscan.Media.Domain.Entities.Scraper;

namespace Mindscan.Media.Adapter.Ports.Scraper.Entities
{
	internal class ParserTestEntity : EntityBase
	{
		public long ParserId { get; set; }
		public string PageUrlPrefix { get; set; }
		public string PageUrl { get; set; }
		public bool Enabled { get; set; }
		public string ResultJson { get; set; }
		public bool Passed { get; set; }
		public DateTime? LastPassedAtUtc { get; set; }
		public int ExecutionsCount { get; set; }

		internal static ParserTestEntity ToEntity(ParserTest test)
		{
			if (test == null)
				return null;

			return new ParserTestEntity
			{
				ParserId = test.ParserId,
				CreatedAtUtc = test.CreatedAtUtc,
				UpdatedAtUtc = test.UpdatedAtUtc,
				Enabled = test.Enabled,
				PageUrl = test.PageUrl.Tail,
				PageUrlPrefix = test.PageUrl.Prefix,
				ResultJson = test.ResultJson,
				ExecutionsCount = test.ExecutionsCount,
				LastPassedAtUtc = test.LastPassedAtUtc
			};
		}

		internal ParserTest FromEntity()
		{
			return ParserTest.GetBuilder()
				.Id(Id)
				.ParserId(ParserId)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.Enabled(Enabled)
				.PageUrl(new Uri($"{PageUrlPrefix}{PageUrl}"))
				.ResultJson(ResultJson)
				.Passed(Passed)
				.LastPassedAtUtc(LastPassedAtUtc)
				.ExecutionsCount(ExecutionsCount)
				.Build();
		}
	}
}