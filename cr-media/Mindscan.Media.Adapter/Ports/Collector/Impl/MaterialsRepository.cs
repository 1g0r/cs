using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Helpers;
using Mindscan.Media.Adapter.Ports.Collector.Entities;
using Mindscan.Media.Adapter.Ports.Scheduler.Entities;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Collector.Impl
{
	internal sealed class MaterialsRepository : PortBase, IMaterialsRepository
	{
		public MaterialsRepository(ILoggerFactory factory, IRepositoryConfig config) : base(factory, config)
		{
		}

		public Material Add(Material material)
		{
			var entity = MaterialEntity.ToEntity(material);

			var query = Sql.Function("collector.insert_material", entity)
				.Parameters(x => new {
					x.CreatedAtUtc,
					x.UpdatedAtUtc,
					x.OriginalUrlPrefix,
					x.OriginalUrlTail,
					x.OriginalUrlHash,
					x.ActualUrlPrefix,
					x.ActualUrlTail,
					x.ActualUrlHash,

					x.SourceId,
					x.ParserId,
					x.FeedUrl,
					x.Title,
					x.Text,
					x.Host,
					x.PublishedAtUtc,

					x.Authors,
					x.Tags,
					x.Images,
					x.Links,
					x.Categories,
					x.Videos,
					x.Pdfs,
					x.Metrics})
				.Build();
			return Connect(cx =>
			{
				var resultEntity = cx.Query<MaterialEntity>(query, entity).SingleOrDefault();
				return resultEntity?.FromEntity();
			});
		}

		public IEnumerable<Tuple<Material, Source>> Find(MaterialFilter filter)
		{
			var sourceFilter = "";
			if (!filter.SourceUrl.IsNullOrWhiteSpace())
			{
				sourceFilter = $"AND LOWER(s.\"Url\") like '%{filter.SourceUrl}%'";
			}

			var feedFilter = "";
			if (!filter.FeedUrl.IsNullOrWhiteSpace())
			{
				feedFilter = $"AND LOWER(md.\"FeedUrl\") like '%{filter.FeedUrl.ToLower()}%'";
			}

			var whereClause = "";
			if (!filter.Url.IsNullOrWhiteSpace())
			{
				whereClause = $"WHERE LOWER(m.\"ActualUrlTail\") like '%{filter.Url.ToLower()}%' " +
							$"OR LOWER(m.\"OriginalUrlTail\") like '%{filter.Url.ToLower()}%' ";
			}

			var query = "SELECT m.*, md.*, s.* " +
						"FROM collector.materials m " +
							$"INNER JOIN collector.materials_data md on md.\"MaterialId\" = m.\"Id\" {feedFilter} " +
							$"INNER JOIN scheduler.sources s on s.\"Id\" = md.\"SourceId\" {sourceFilter} " +
						$"{whereClause}" +
						$"ORDER BY m.\"CreatedAtUtc\" DESC LIMIT {filter.Count} OFFSET {filter.Offset};";

			return Connect(cx =>
			{
				return cx.Query<MaterialEntity, SourceEntity, Tuple<Material, Source>>(
					query,
					(materialEntity, sourceEntity) =>
					{
						if (materialEntity == null || sourceEntity == null)
							return null;
						return new Tuple<Material, Source>(materialEntity.FromEntity(), sourceEntity.FromEntity());
					}
				).Where(x => x!= null);
			});
		}

		public bool Exists(Material material)
		{
			var entity = MaterialEntity.ToEntity(material);

			var query = Sql.Function("collector.exist_material", entity)
				.Parameters(x => new
				{
					x.OriginalUrlHash,
					x.ActualUrlHash
				})
				.Build();
			return Connect(cx => cx.ExecuteScalar<bool>(query, entity));
		}

		public bool Exists(NormalizedUrl url)
		{
			var hash = url.ComputeMd5Hash();
			var query = Sql.Function("collector.exist_url", url)
				.Parameters(x => new { hash })
				.Build();

			return Connect(cx => cx.ExecuteScalar<bool>(query, new { hash })); 
		}

		public IEnumerable<NormalizedUrl> FilterNotCollected(IEnumerable<NormalizedUrl> urls)
		{
			var hashes = urls.Distinct().ToDictionary(x => x.ComputeMd5Hash());
			var query = Sql.Function("collector.filter_hashes", hashes.Keys)
				.Parameters(x => new {Hashes = x})
				.Build();

			var result = Connect(cx => cx.Query<string>(query, new {Hashes = hashes.Keys.ToArray() }));

			return result
				.Select(x => hashes[x]);
		}
	}
}
