using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Messages.Collector
{
	public sealed class MaterialData
	{
		public Uri OriginalUrl { get; set; }
		public Uri ActualUrl { get; set; }
		public DateTimeOffset PublishedAt { get; set; }
		public string Title { get; set; }
		public List<string> Paragraphs { get; } = new List<string>();
		public SourceData Source { get; set; }
		public long? ParserId { get; set; }
		public Uri FeedUrl { get; set; }

		public int SharesCount { get; set; }
		public int CommentsCount { get; set; }
		public int LikesCount { get; set; }
		public string MediaCategory { get; set; }

		public List<AuthorData> Authors { get; } = new List<AuthorData>();
		public List<string> Categories { get; } = new List<string>();
		public List<string> Tags { get; } = new List<string>();
		public List<LinkData> Images { get; set; } = new List<LinkData>();
		public List<LinkData> Links { get; } = new List<LinkData>();
		public List<LinkData> Videos { get; } = new List<LinkData>();
		public List<LinkData> Pdfs { get; } = new List<LinkData>();

		// Used Only for tests
		public Uri Url { get; set; }

		public Material ToMaterial()
		{
			return Material.GetBuilder()
				.SourceId(Source.Id)
				.ParserId(ParserId)
				.FeedUrl(FeedUrl)
				.OriginalUrl(OriginalUrl)
				.ActualUrl(ActualUrl)
				.Title(Title)
				.Text(string.Join(Environment.NewLine, Paragraphs))
				.Host(Source.Url)
				.PublishedAtUtc(PublishedAt.UtcDateTime)

				.Authors(MapAuthors(Authors))
				.Tags(Tags)
				.Images(MapLinks(Images))
				.Links(MapLinks(Links))
				.Categories(Categories)
				.Videos(MapLinks(Videos))
				.Pdfs(MapLinks(Pdfs))
				.Metrics(Map(this))
				.Build();

		}
		private static IEnumerable<Author> MapAuthors(List<AuthorData> authors)
		{
			if (authors == null)
				return Enumerable.Empty<Author>();

			return authors
				.Where(x => x?.Name != null)
				.Select(x => Author.GetBuilder().Name(x.Name).Url(x.Url).Build());
		}

		private static IEnumerable<Link> MapLinks(List<LinkData> links)
		{
			if (links == null || links.Count == 0)
				return Enumerable.Empty<Link>();

			return links
				.Where(x => x?.Url != null)
				.Select(x => Link.GetBuilder().Url(x.Url).Title(x.Title).Build());
		}

		private static Metrics Map(MaterialData data)
		{
			var test = data.CommentsCount + data.SharesCount + data.LikesCount;
			if (test != 0)
			{
				return Metrics
					.GetBuilder()
					.CommentsCount(data.CommentsCount)
					.LikesCount(data.LikesCount)
					.SharesCount(data.SharesCount)
					.Build();
			}

			return null;
		}
	}

	public sealed class AuthorData
	{
		public string Name { get; set; }
		public Uri Url { get; set; }
	}

	public sealed class SourceData
	{
		public long Id { get; set; }
		public Uri Url { get; set; }
		public SourceType Type { get; set; }
		public string Name { get; set; }
	}

	public sealed class LinkData
	{
		public Uri Url { get; set; }
		public string Title { get; set; }
	}
}