using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Ports.Scheduler.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal class DbFeedsRepository: PortBase, IFeedsRepository
	{
		public DbFeedsRepository(ILoggerFactory factory, IRepositoryConfig config) : base(factory, config)
		{
		}

		public Feed Add(long sourceId, Feed feed)
		{
			if (feed == null)
				return null;

			var entity = FeedEntity.ToEntity(feed);
			entity.CreatedAtUtc = entity.UpdatedAtUtc = DateTime.UtcNow;
			entity.SourceId = sourceId;
			return Connect((cx, tr) =>
			{
				var result = cx.Query<FeedEntity>(
					"insert into scheduler.feeds (\"SourceId\", \"FeedType\", \"CreatedAtUtc\", \"UpdatedAtUtc\", \"OriginalUrlPrefix\", \"OriginalUrl\", \"ActualUrlPrefix\", \"ActualUrl\", \"Encoding\", \"Description\", \"AdditionalInfo\") " +
					"values (@SourceId, @FeedType, @CreatedAtUtc, @UpdatedAtUtc, @OriginalUrlPrefix, @OriginalUrl, @ActualUrlPrefix, @ActualUrl, @Encoding,  @Description, @AdditionalInfo) " +
					"returning *;",
					entity,
					tr
				).Single().FromEntity(feed.Source);

				return result;
			});
		}

		public Feed Find(long id)
		{
			return Connect(cx =>
			{
				return cx.Query<FeedEntity, SourceEntity, Feed>(
					"select * " +
					"from scheduler.feeds f " +
					"inner join scheduler.sources s on s.\"Id\" = f.\"SourceId\" " +
					"where f.\"Id\" = @id;",
					(feedEntity, sourceEntity) =>
					{
						return feedEntity.FromEntity(sourceEntity.FromEntity());
					},
					new { id }
				).Single();
			});
		}

		public IEnumerable<Feed> Find(FeedFilter filter)
		{
			string filterClause = " ";
			var leadingWildcard = filter.LeadingWildcard ? "%" : "";
			var followingWildcard = filter.FollowingWildcard ? "%" : "";
			if (!string.IsNullOrWhiteSpace(filter.ActualUrl))
			{
				filterClause += $"AND TRIM(TRAILING '/' FROM LOWER(\"ActualUrl\")) LIKE '{leadingWildcard + filter.ActualUrl.ToLower() + followingWildcard}'";
			}
			else if (!string.IsNullOrWhiteSpace(filter.OriginalUrl))
			{
				filterClause += $"AND TRIM(TRAILING '/' FROM LOWER(\"OriginalUrl\")) LIKE '{leadingWildcard + filter.OriginalUrl.ToLower() + followingWildcard}'";
			}

			if (!string.IsNullOrWhiteSpace(filter.Encoding))
			{
				filterClause += $"AND \"Encoding\"='{filter.Encoding}'";
			}
			filterClause += " ";

			var triggersFilter = " ";
			if (filter.TriggersFilter.HasValue)
			{
				switch (filter.TriggersFilter)
				{
					case TriggersFilter.Disabled:
						triggersFilter += "AND tt.\"Enabled\" = false ";
						break;
					case TriggersFilter.Enabled:
						triggersFilter += "AND tt.\"Enabled\" = true ";
						break;
				}
			}

			var sql = "SELECT * " +
					"FROM scheduler.sources s " +
					"INNER JOIN ( " +
						"SELECT distinct ff.* " +
						"FROM scheduler.feeds ff " +
							"LEFT JOIN scheduler.triggers tt on tt.\"FeedId\" = ff.\"Id\" " +
						$"WHERE ff.\"SourceId\" = @SourceId {filterClause} {triggersFilter} " +
						$"ORDER BY ff.\"OriginalUrl\" LIMIT {filter.Count} OFFSET {filter.Offset}) f on f.\"SourceId\" = s.\"Id\" " +
					"LEFT JOIN scheduler.triggers t on t.\"FeedId\" = f.\"Id\" " +
					"ORDER BY f.\"OriginalUrl\"; ";

			return Connect(cx =>
			{
				var feedsDistinct = new Dictionary<long, Feed>();
				return cx.Query<SourceEntity, FeedEntity, TriggerEntity, Feed>(
					sql,
					(sourceEntity, feedEntity, triggerEntity) =>
					{

						if (feedEntity != null)
						{
							Feed feed;
							if (!feedsDistinct.TryGetValue(feedEntity.Id, out feed))
							{
								feed = feedEntity.FromEntity(sourceEntity.FromEntity());
								feedsDistinct.Add(feed.Id, feed);
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
					filter
				).Where(x => x != null).Distinct();
			});
		}

		public int Delete(Feed feed)
		{
			if (feed == null)
				return 0;

			return Connect((cx, tran) =>
			{
				// Cascade delete
				cx.Execute("delete from scheduler.feeds where \"Id\"=@Id", feed, tran);
				return 1;
			});
		}

		public Feed Update(Feed feed)
		{
			var entity = FeedEntity.ToEntity(feed);
			return Connect(cx =>
			{
				var result = cx.Query<FeedEntity>(
						"update scheduler.feeds " +
						"set \"OriginalUrlPrefix\"=@OriginalUrlPrefix, \"OriginalUrl\"=@OriginalUrl, \"FeedType\"=@FeedType, " +
						"\"ActualUrlPrefix\"=@ActualUrlPrefix, \"ActualUrl\"=@ActualUrl, \"Encoding\"=@Encoding, " +
						"\"Description\"=@Description, \"AdditionalInfo\"=@AdditionalInfo, \"UpdatedAtUtc\"=@NewUpdatedDate " +
						"where \"Id\"=@Id and \"UpdatedAtUtc\"=@UpdatedAtUtc " +
						"returning *;",
						new
						{
							entity.OriginalUrlPrefix,
							entity.OriginalUrl,
							entity.FeedType,
							entity.ActualUrlPrefix,
							entity.ActualUrl,
							entity.Encoding,
							entity.Description,
							entity.AdditionalInfo,
							NewUpdatedDate = DateTime.UtcNow,
							entity.Id,
							entity.UpdatedAtUtc
						}
					)
					.Single().FromEntity(feed.Source);

				result.Triggers.AddRange(feed.Triggers);
				return result;
			});
		}

		public Feed UpdateActualUrl(Feed feed)
		{
			return Connect(cx =>
			{
				var entity = FeedEntity.ToEntity(feed);
				var result = cx.Query<FeedEntity>(
					"update scheduler.feeds " +
						"set \"ActualUrlPrefix\"=@ActualUrlPrefix, \"ActualUrl\"=@ActualUrl, \"UpdatedAtUtc\"=@NewUpdatedDate " +
					"where \"Id\"=@Id and \"UpdatedAtUtc\"=@UpdatedAtUtc " +
					"returning *;",
					new
					{
						entity.ActualUrlPrefix,
						entity.ActualUrl,
						NewUpdatedDate = DateTime.UtcNow,
						entity.Id,
						entity.UpdatedAtUtc
					}
				).SingleOrDefault();

				return result?.FromEntity(feed.Source);
			});

		}
	}
}