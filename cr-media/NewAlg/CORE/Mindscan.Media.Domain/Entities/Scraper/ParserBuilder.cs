using System;
using System.Diagnostics;
using Mindscan.Media.Domain.Const;

namespace Mindscan.Media.Domain.Entities.Scraper
{
	[DebuggerStepThrough]
	public sealed class ParserBuilder : EntityBuilderBase<Parser, ParserBuilder>
	{
		internal ParserBuilder(Parser parser):base(parser)
		{
			
		}

		public ParserBuilder SourceId(long value)
		{
			Entity.SourceId = value;
			return this;
		}

		public ParserBuilder Host(Uri value)
		{
			AssertRequired(value, nameof(Host));
			Entity.Host = NormalizedUrl.Build(value);
			return this;
		}

		public ParserBuilder Path(string value)
		{
			Entity.Path = value?.Trim().ToLower();
			return this;
		}
		public ParserBuilder Encoding(string value)
		{
			Entity.Encoding = string.IsNullOrWhiteSpace(value) ? 
				GlobalDefaults.Encoding : 
				value.ToLower();
			return this;
		}

		public ParserBuilder UseBrowser(bool value)
		{
			Entity.UseBrowser = value;
			return this;
		}

		public ParserBuilder Tag(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				Entity.Tag = value;
			}

			return this;
		}

		public ParserBuilder Json(string value)
		{
			Entity.Json = value;
			return this;
		}

		public ParserBuilder AdditionalInfo(string value)
		{
			Entity.AdditionalInfo = value;
			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.SourceId, nameof(SourceId));
			AssertRequired(Entity.Host, nameof(Host));
			if (string.IsNullOrWhiteSpace(Entity.Encoding))
			{
				Entity.Encoding = GlobalDefaults.Encoding;
			}

			if (string.IsNullOrWhiteSpace(Entity.Path))
			{
				Entity.Path = null;
			}

			if (string.IsNullOrWhiteSpace(Entity.Tag))
			{
				Entity.Tag = null;
			}
		}
	}
}