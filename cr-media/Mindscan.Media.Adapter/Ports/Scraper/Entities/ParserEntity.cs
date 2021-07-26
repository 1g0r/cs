using System;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Utils.Helpers;

namespace Mindscan.Media.Adapter.Ports.Scraper.Entities
{
	internal class ParserEntity : EntityBase
	{
		public long SourceId { get; set; }
		public string Host { get; protected internal set; }
		public string Path { get; set; }
		public string Encoding { get; set; }
		public bool UseBrowser { get; set; }
		public string Tag { get; set; }
		public string AdditionalInfo { get; set; }
		public ParserJsonEntity Json { get; set; }

		public static ParserEntity ToEntity(Parser parser)
		{
			if (parser == null)
				return null;

			return new ParserEntity
			{
				Id = parser.Id,
				CreatedAtUtc = parser.CreatedAtUtc,
				UpdatedAtUtc = parser.UpdatedAtUtc,
				SourceId = parser.SourceId,
				Host = parser.Host.Host,
				Path = parser.Path.IsNullOrWhiteSpace() ? null : parser.Path.ToLower(),
				Encoding = parser.Encoding ?? "utf-8",
				UseBrowser = parser.UseBrowser,
				Tag = parser.Tag,
				AdditionalInfo = parser.AdditionalInfo,
				Json = new ParserJsonEntity
				{
					Id = parser.Id,
					Value = parser.Json
				}
			};
		}

		public Parser FromEntity()
		{
			return Parser.GetBuilder()
				.Id(Id)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.SourceId(SourceId)
				.Host(new Uri($"http://{Host}"))
				.Path(Path)
				.Encoding(Encoding)
				.UseBrowser(UseBrowser)
				.Tag(Tag)
				.AdditionalInfo(AdditionalInfo)
				.Json(Json?.Value)
				.Build();
		}
	}
}
