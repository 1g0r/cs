using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Ports.Scheduler.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal class DbSourcesRepository : PortBase, ISourcesRepository
	{
		public DbSourcesRepository(ILoggerFactory factory, IRepositoryConfig config) : base(factory, config)
		{
		}

		public bool Exists(Source source)
		{
			var entity = SourceEntity.ToEntity(source);
			return Connect(cx => cx.ExecuteScalar<bool>(
				"select exists(select 1 from scheduler.sources where \"Url\"=@Url);",
				entity));
		}

		public Source Add(Source source)
		{
			if (source == null)
				return null;

			if (string.IsNullOrWhiteSpace(source.Name))
				throw new RequiredFieldException("Name");

			var entity = SourceEntity.ToEntity(source);
			entity.CreatedAtUtc = entity.UpdatedAtUtc = DateTime.UtcNow;
			return Connect(cx =>
			{
				var newSource = cx.Query<SourceEntity>(
					"insert into scheduler.sources (\"Url\", \"SourceType\", \"Name\", \"CreatedAtUtc\", \"UpdatedAtUtc\", \"AdditionalInfo\") " +
					"values (@Url, @SourceType, @Name, @CreatedAtUtc, @UpdatedAtUtc, @AdditionalInfo) " +
					"returning *;",
					entity
				).Single().FromEntity();
				return newSource;
			});
		}

		public Source Find(long id)
		{
			return Connect(cx =>
			{
				Source result = null;
				Source resultClone = null;
				var feedsDistinct = new Dictionary<long, Feed>();

				var feeds = cx.Query<SourceEntity, FeedEntity, TriggerEntity, Feed>(
					"select * from scheduler.sources s " +
					"left join scheduler.feeds f on f.\"SourceId\" = s.\"Id\" " +
					"left join scheduler.triggers t on t.\"FeedId\" = f.\"Id\" " +
					"where s.\"Id\" = @id;",
					(sourceEntity, feedEntity, triggerEntity) =>
					{
						if (result == null)
						{
							result = sourceEntity.FromEntity();
							resultClone = sourceEntity.FromEntity();
						}

						if (feedEntity != null)
						{
							Feed feed;
							if (!feedsDistinct.TryGetValue(feedEntity.Id, out feed))
							{
								feed = feedEntity.FromEntity(resultClone);
								feedsDistinct.Add(feedEntity.Id, feed);
							}

							var trigger = triggerEntity?.FromEntity();
							if (trigger != null)
							{
								feed.Triggers.Add(trigger);
							}
							return feed;
						}
						return null;

					},
					new { id }
				).Where(f => f != null).Distinct();

				result?.Feeds.AddRange(feeds);
				return result;
			});
		}

		public IEnumerable<Source> Find(SourceFilter criteria)
		{
			var sortOrder = criteria.SortAsc ? "ASC" : "DESC";
			var leadingWildcard = criteria.LeadingWildcard ? "%" : "";
			var followingWildcard = criteria.FollowingWildcard ? "%" : "";
			var likePattern = criteria.UrlPrefix.IsNullOrWhiteSpace() ? "%" : 
				$"{leadingWildcard + (criteria.UrlPrefix) + followingWildcard}";
			
			var query = "SELECT * " +
						"FROM (SELECT * " +
						"FROM scheduler.sources " +
						$"WHERE TRIM(TRAILING '/' FROM \"Url\") LIKE '{likePattern}' " +
						$"ORDER BY \"Url\" {sortOrder} " +
						"LIMIT " + criteria.Count + " OFFSET " + criteria.Offset + ") AS s " +
						"LEFT JOIN scheduler.feeds AS f ON f.\"SourceId\" = s.\"Id\" " +
						$"ORDER BY \"Url\" {sortOrder}";

			return Connect(cx =>
			{
				var dictionary = new Dictionary<long, Source>();
				cx.Query<SourceEntity, FeedEntity, Source>(
					query,
					(sourceEntity, feedEntity) =>
					{
						var sourceClone = sourceEntity.FromEntity();
						if (dictionary.ContainsKey(sourceEntity.Id))
						{
							if (feedEntity != null)
							{
								dictionary[sourceEntity.Id].Feeds.Add(
									feedEntity.FromEntity(sourceClone));
							}
						}
						else
						{
							var source = sourceEntity.FromEntity();
							if (feedEntity != null)
							{
								source.Feeds.Add(feedEntity.FromEntity(sourceClone));
							}

							dictionary.Add(source.Id, source);
						}
						return dictionary[sourceEntity.Id];
					}, criteria);
				
				return dictionary.Values.Distinct();
			});
		}

		public int Delete(Source source)
		{
			return Connect((cx, tran) =>
			{
				// Cascade remove
				cx.Execute("delete from scheduler.sources where \"Id\"=@id and \"UpdatedAtUtc\"=@updatedAtUtc;", source, tran);
				return 1;
			});
		}

		public Source Update(Source source)
		{
			var entity = SourceEntity.ToEntity(source);
			return Connect(cx =>
			{
				var result = cx
					.Query<SourceEntity>(
						"update scheduler.sources " +
						"set \"Url\"=@Url, \"SourceType\"=@SourceType, \"Name\"=@Name, \"AdditionalInfo\"=@AdditionalInfo, \"UpdatedAtUtc\"=@NewUpdatedDate " +
						"where \"Id\"=@Id and \"UpdatedAtUtc\"=@UpdatedAtUtc " +
						"returning *;",
						new
						{
							entity.Id,
							entity.Url,
							entity.SourceType,
							entity.Name,
							entity.AdditionalInfo,
							entity.UpdatedAtUtc,
							NewUpdatedDate = DateTime.UtcNow
						})
					.Single().FromEntity();

				result?.Feeds.AddRange(source.Feeds);
				return result;
			});
		}
	}
}
