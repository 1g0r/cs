using System;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class LinkBuilder : BuilderBase<Link>
	{
		internal LinkBuilder(Link entity): base(entity)
		{
			
		}

		public LinkBuilder Url(Uri value)
		{
			AssertRequired(value, nameof(Url));
			Entity.Url = value;
			return this;
		}

		public LinkBuilder Title(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				Entity.Title = value;
			}

			return this;
		}


		protected override void BuildInternal()
		{
			AssertRequired(Entity.Url, nameof(Url));
		}
	}
}