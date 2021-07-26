using System;
using System.Diagnostics;
using System.IO;
using Mindscan.Media.Domain.Const;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	[DebuggerStepThrough]
	public class FeedBuilder : EntityBuilderBase<Feed, FeedBuilder>
	{
		internal FeedBuilder():base(new Feed())
		{
			
		}

		public FeedBuilder OriginalUrl(Uri value)
		{
			AssertRequired(value, nameof(OriginalUrl));
			Entity.OriginalUrl = NormalizedUrl.Build(value);
			return this;
		}

		public FeedBuilder ActualUrl(Uri value)
		{
			if (value != null)
			{
				Entity.ActualUrl = NormalizedUrl.Build(value);
			}
			return this;
		}

		public FeedBuilder Type(FeedType value)
		{
			Entity.Type = value;
			return this;
		}

		public FeedBuilder Encoding(string value)
		{
			Entity.Encoding = string.IsNullOrWhiteSpace(value) 
				? GlobalDefaults.Encoding
				: value.ToLower();

			return this;
		}

		public FeedBuilder Description(string value)
		{
			Entity.Description = string.IsNullOrWhiteSpace(value) ? null : value;
			return this;
		}

		public FeedBuilder AdditionalInfo(string value)
		{
			Entity.AdditionalInfo = string.IsNullOrWhiteSpace(value) ? null : value;
			return this;
		}

		public FeedBuilder Source(Source value)
		{
			Entity.Source = value;
			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.OriginalUrl, nameof(OriginalUrl));
			if (Entity.ActualUrl != null && Entity.OriginalUrl.Value == Entity.ActualUrl.Value)
				throw new InvalidDataException("ActualUrl should differ from OriginalUrl.");

			//Set Defaults
			if (string.IsNullOrWhiteSpace(Entity.Encoding))
			{
				Entity.Encoding = GlobalDefaults.Encoding;
			}

			if (string.IsNullOrWhiteSpace(Entity.Description))
			{
				Entity.Description = null;
			}

			if (string.IsNullOrWhiteSpace(Entity.AdditionalInfo))
			{
				Entity.AdditionalInfo = null;
			}
		}
	}
}
