using System;
using System.Collections.Generic;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class MaterialBuilder : EntityBuilderBase<Material, MaterialBuilder>
	{
		internal MaterialBuilder(Material material) : base(material)
		{
		}

		public MaterialBuilder SourceId(long value)
		{
			AssertRequired(value, nameof(SourceId));
			Entity.SourceId = value;
			return this;
		}

		public MaterialBuilder ParserId(long? value)
		{
			Entity.ParserId = value;
			return this;
		}

		public MaterialBuilder OriginalUrl(Uri value)
		{
			AssertRequired(value, nameof(OriginalUrl));
			Entity.OriginalUrl = NormalizedUrl.Build(value);
			return this;
		}

		public MaterialBuilder ActualUrl(Uri value)
		{
			AssertRequired(value, nameof(ActualUrl));
			Entity.ActualUrl = NormalizedUrl.Build(value);
			return this;
		}

		public MaterialBuilder FeedUrl(Uri value)
		{
			if (value != null)
			{
				Entity.FeedUrl = NormalizedUrl.Build(value);
			}

			return this;
		}

		public MaterialBuilder Title(string value)
		{
			AssertRequired(value, nameof(Title));
			Entity.Title = value;
			return this;
		}

		public MaterialBuilder Text(string value)
		{
			AssertRequired(value, nameof(Text));
			Entity.Text = value;
			return this;
		}

		public MaterialBuilder Host(Uri value)
		{
			AssertRequired(value, nameof(Host));
			Entity.Host = NormalizedUrl.Build(value);
			return this;
		}

		public MaterialBuilder PublishedAtUtc(DateTime value)
		{
			AssertRequired(value, nameof(PublishedAtUtc));
			AssertUtc(value, nameof(PublishedAtUtc));
			Entity.PublishedAtUtc = value;
			return this;
		}

		public MaterialBuilder Authors(IEnumerable<Author> authors)
		{
			if (authors != null)
			{
				Entity.Authors.Clear();
				Entity.Authors.AddRange(authors);
			}

			return this;
		}

		public MaterialBuilder Tags(IEnumerable<string> tags)
		{
			if (tags != null)
			{
				Entity.Tags.Clear();
				Entity.Tags.AddRange(tags);
			}
			return this;
		}

		public MaterialBuilder Images(IEnumerable<Link> images)
		{
			if (images != null)
			{
				Entity.Images.Clear();
				Entity.Images.AddRange(images);
			}

			return this;
		}

		public MaterialBuilder Links(IEnumerable<Link> links)
		{
			if (links != null)
			{
				Entity.Links.Clear();
				Entity.Links.AddRange(links);
			}

			return this;
		}

		public MaterialBuilder Categories(IEnumerable<string> categories)
		{
			if (categories != null)
			{
				Entity.Categories.Clear();
				Entity.Categories.AddRange(categories);
			}

			return this;
		}

		public MaterialBuilder Videos(IEnumerable<Link> videos)
		{
			if (videos != null)
			{
				Entity.Videos.Clear();
				Entity.Videos.AddRange(videos);
			}

			return this;
		}

		public MaterialBuilder Pdfs(IEnumerable<Link> pdfs)
		{
			if (pdfs != null)
			{
				Entity.Pdfs.Clear();
				Entity.Pdfs.AddRange(pdfs);
			}

			return this;
		}

		public MaterialBuilder Metrics(Metrics value)
		{
			Entity.Metrics = value;
			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.SourceId, nameof(SourceId));
			AssertRequired(Entity.OriginalUrl, nameof(OriginalUrl));
			AssertRequired(Entity.ActualUrl, nameof(ActualUrl));
			AssertRequired(Entity.Title, nameof(Title));
			AssertRequired(Entity.Text, nameof(Text));
			AssertRequired(Entity.Host, nameof(Host));
			AssertRequired(Entity.PublishedAtUtc, nameof(PublishedAtUtc));
			AssertUtc(Entity.PublishedAtUtc, nameof(PublishedAtUtc));
		}
	}
}