using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.Adapter.Helpers;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Utils.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.Adapter.Ports.Collector.Entities
{
	internal sealed class MaterialEntity : EntityBase
	{
		public string OriginalUrlPrefix { get; set; }
		public string OriginalUrlTail { get; set; }
		public string OriginalUrlHash { get; set; }

		public string ActualUrlPrefix { get; set; }
		public string ActualUrlTail { get; set; }
		public string ActualUrlHash { get; set; }

		public long SourceId { get; set; }
		public long? ParserId { get; set; }
		public string FeedUrl { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public string Host { get; set; }
		public DateTime PublishedAtUtc { get; set; }
		
		public string Authors { get; set; }
		public string Tags { get; set; }
		public string Images { get; set; }
		public string Links { get; set; }
		public string Categories { get; set; }
		public string Videos { get; set; }
		public string Pdfs { get; set; }
		public string Metrics { get; set; }

		public static MaterialEntity ToEntity(Material material)
		{
			if (material == null)
				return null;
			return new MaterialEntity
			{
				Id = material.Id,
				CreatedAtUtc = material.CreatedAtUtc,
				UpdatedAtUtc = material.UpdatedAtUtc,
				OriginalUrlPrefix = material.OriginalUrl.Prefix,
				OriginalUrlTail = material.OriginalUrl.Tail,
				OriginalUrlHash = material.OriginalUrl.ComputeMd5Hash(),
				ActualUrlPrefix = material.ActualUrl.Prefix,
				ActualUrlTail = material.ActualUrl.Tail,
				ActualUrlHash = material.ActualUrl.ComputeMd5Hash(),
				SourceId = material.SourceId,
				ParserId = material.ParserId,
				FeedUrl = material.FeedUrl.ToString(),
				Title = material.Title,
				Text = material.Text,
				Host = material.Host.Tail,
				PublishedAtUtc = material.PublishedAtUtc,

				Authors = SerializeAuthors(material.Authors),
				Tags = SerializeStrings(material.Tags),
				Images = SerializeLinks(material.Images),
				Links = SerializeLinks(material.Links),
				Categories = SerializeStrings(material.Categories),
				Videos = SerializeLinks(material.Videos),
				Pdfs = SerializeLinks(material.Pdfs),
				Metrics = SerializeMetrics(material.Metrics)
			};
		}

		public Material FromEntity()
		{
			return Material.GetBuilder()
				.Id(Id)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.OriginalUrl(new Uri(OriginalUrlPrefix + OriginalUrlTail))
				.ActualUrl(new Uri(ActualUrlPrefix + ActualUrlTail))
				.FeedUrl(FeedUrl != null ? new Uri(FeedUrl) : null)
				.SourceId(SourceId)
				.ParserId(ParserId)
				.Title(Title)
				.Text(Text)
				.Host(new Uri($"http://{Host}"))
				.PublishedAtUtc(PublishedAtUtc)
				
				.Authors(DeserializeAuthors(Authors))
				.Tags(DeserializeStrings(Tags))
				.Images(DeserializeLinks(Images))
				.Links(DeserializeLinks(Links))
				.Categories(DeserializeStrings(Categories))
				.Videos(DeserializeLinks(Videos))
				.Pdfs(DeserializeLinks(Pdfs))
				.Metrics(DeserializeMetrics(Metrics))
				.Build();
		}

		private static string SerializeLinks(List<Link> links)
		{
			if (!links.IsNullOrEmpty())
			{
				return JsonConvert.SerializeObject(links.Select(x => new LinkEntity
				{
					Url = x.Url,
					Title = x.Title
				}));
			}

			return null;
		}

		private static IEnumerable<Link> DeserializeLinks(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Enumerable.Empty<Link>();
			return JsonConvert.DeserializeObject<LinkEntity[]>(value)
				.Select(x => Link.GetBuilder().Url(x.Url).Title(x.Title).Build());
		}

		private static string SerializeStrings(List<string> links)
		{
			if (!links.IsNullOrEmpty())
			{
				return JsonConvert.SerializeObject(links);
			}

			return null;
		}

		private static IEnumerable<string> DeserializeStrings(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Enumerable.Empty<string>();
			return JsonConvert.DeserializeObject<string[]>(value);
		}

		private static string SerializeMetrics(Metrics value)
		{
			if (value != null)
			{
				var sum = value.CommentsCount + value.LikesCount + value.SharesCount;
				if (sum > 0)
				{
					return JsonConvert.SerializeObject(value);
				}
			}
			return null;
		}

		private static Metrics DeserializeMetrics(string value)
		{
			if (value.IsNullOrEmpty())
				return null;
			return JsonConvert.DeserializeObject<Metrics>(value);
		}

		private static string SerializeAuthors(List<Author> authors)
		{
			if (!authors.IsNullOrEmpty())
			{
				var entities = authors.Select(x => new AuthorEntity
				{
					Name = x.Name,
					Url = x.Url?.Value
				});
				return JsonConvert.SerializeObject(entities);
			}

			return null;
		}

		private static IEnumerable<Author> DeserializeAuthors(string json)
		{
			if(json.IsNullOrWhiteSpace())
				return new List<Author>();
			var entyties = JsonConvert.DeserializeObject<AuthorEntity[]>(json);
			return entyties
				.Select(x => Author.GetBuilder().Name(x.Name).Url(x.Url).Build());
		}
	}
}
