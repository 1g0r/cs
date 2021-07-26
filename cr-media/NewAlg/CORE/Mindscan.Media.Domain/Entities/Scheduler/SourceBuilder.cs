using System;
using System.Diagnostics;
using Mindscan.Media.Domain.Enums;
using Mindscan.Media.Domain.Exceptions;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	[DebuggerStepThrough]
	public class SourceBuilder : EntityBuilderBase<Source, SourceBuilder>
	{
		internal SourceBuilder(Source entity):base(entity)
		{
			
		}
		public SourceBuilder Url(Uri value)
		{
			AssertRequired(value, nameof(Url));
			Entity.Url = NormalizedUrl.Build(value);
			return this;
		}

		public SourceBuilder Type(SourceType value)
		{
			Entity.Type = value;
			return this;
		}

		public SourceBuilder Name(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new RequiredFieldException(nameof(Name));

			Entity.Name = value;
			return this;
		}

		public SourceBuilder AdditionalInfo(string value)
		{
			Entity.AdditionalInfo = value;
			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.Url, nameof(Url));
			AssertRequired(Entity.Name, nameof(Name));
		}
	}
}