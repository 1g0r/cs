using System;
using System.Diagnostics;
using Mindscan.Media.Domain.Exceptions;

namespace Mindscan.Media.Domain.Entities.Scraper
{
	[DebuggerStepThrough]
	public sealed class ParserTestBuilder : EntityBuilderBase<ParserTest, ParserTestBuilder>
	{
		internal ParserTestBuilder(ParserTest entity): base(entity)
		{

		}

		public ParserTestBuilder ParserId(long value)
		{
			Entity.ParserId = value;
			return this;
		}
		public ParserTestBuilder PageUrl(Uri value)
		{
			AssertRequired(value, nameof(PageUrl));
			Entity.PageUrl = NormalizedUrl.Build(value);
			return this;
		}

		public ParserTestBuilder Enabled(bool value)
		{
			Entity.Enabled = value;
			return this;
		}

		public ParserTestBuilder ResultJson(string value)
		{
			AssertRequired(value, nameof(ResultJson));
			Entity.ResultJson = value;
			return this;
		}

		public ParserTestBuilder Passed(bool value)
		{
			Entity.Passed = value;
			return this;
		}

		public ParserTestBuilder LastPassedAtUtc(DateTime? value)
		{
			if (value.HasValue && value.Value != DateTime.MinValue)
			{
				if (value.Value.Kind != DateTimeKind.Utc)
				{
					throw new NotUtcTimeZoneException(nameof(LastPassedAtUtc));
				}
				Entity.LastPassedAtUtc = value;
			}
			return this;
		}

		public ParserTestBuilder ExecutionsCount(int value)
		{
			if (value > 0)
			{
				Entity.ExecutionsCount = value;
			}
			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.PageUrl, nameof(PageUrl));
			AssertRequired(Entity.ResultJson, nameof(ResultJson));
		}
	}
}