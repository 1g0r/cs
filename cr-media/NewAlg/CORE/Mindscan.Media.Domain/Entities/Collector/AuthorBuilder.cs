using System;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class AuthorBuilder :BuilderBase<Author>
	{
		public AuthorBuilder(Author entity) : base(entity)
		{
		}

		public AuthorBuilder Name(string value)
		{
			AssertRequired(value, nameof(Name));
			Entity.Name = value;
			return this;
		}

		public AuthorBuilder Url(Uri url)
		{
			if (url != null)
			{
				Entity.Url = NormalizedUrl.Build(url);
			}

			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.Name, nameof(Name));
		}
	}
}
