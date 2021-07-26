using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Entities
{
	internal class SourceEntity : EntityBase
	{
		public string Url { get; set; }
		public string SourceType { get; set; }
		public string Name { get; set; }
		public string AdditionalInfo { get; set; }

		public static SourceEntity ToEntity(Source source)
		{
			return new SourceEntity
			{
				Id = source.Id,
				SourceType = source.Type.ToString("G"),
				Name = source.Name,
				Url = source.Url.Tail,
				AdditionalInfo = source.AdditionalInfo,
				UpdatedAtUtc = source.UpdatedAtUtc,
				CreatedAtUtc = source.CreatedAtUtc
			};
		}

		public Source FromEntity()
		{
			return Source.GetBuilder()
				.Id(Id)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.Type((SourceType)Enum.Parse(typeof(SourceType), SourceType))
				.Url(new Uri($"http://{Url}"))
				.Name(Name)
				.AdditionalInfo(AdditionalInfo)
				.Build();
		}
	}
}