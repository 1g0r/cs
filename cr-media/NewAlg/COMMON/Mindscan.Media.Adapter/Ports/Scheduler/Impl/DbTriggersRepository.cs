using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Helpers;
using Mindscan.Media.Adapter.Ports.Scheduler.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal sealed class DbTriggersRepository : PortBase, ITriggersRepository
	{
		public DbTriggersRepository(ILoggerFactory factory, IRepositoryConfig repositoryConfig)
			:base(factory, repositoryConfig)
		{
		}

		public bool Exists(Trigger trigger)
		{
			return Connect(cx => cx.ExecuteScalar<bool>(
				"select exists(select 1 from scheduler.triggers where \"FeedId\"=@FeedId and \"RoutingKey\"=@RoutingKey);",
				trigger));
		}

		public Trigger Add(Trigger trigger)
		{
			var entity = TriggerEntity.ToEntity(trigger);
			return Connect(cx =>
			{
				return cx
					.Query<TriggerEntity>(
						"insert into scheduler.triggers " +
						"(\"FeedId\", \"RoutingKey\", \"VirtualHost\", \"Enabled\", \"RepeatInterval\", \"CreatedAtUtc\", \"StartAtUtc\", \"FireTimeUtc\", \"FireCount\", \"Payload\", \"UpdatedAtUtc\") " +
						"values (@FeedId, @RoutingKey, @VirtualHost, @Enabled, @RepeatInterval, @CreatedAtUtc, @StartAtUtc, @FireTimeUtc, 0, @Payload, @UpdatedAtUtc) " +
						"returning *;",
						entity)
					.Single().FromEntity();
			});
		}

		public IEnumerable<Trigger> Add(Source source, Trigger trigger)
		{
			var entity = TriggerEntity.ToEntity(trigger);
			var query = "INSERT INTO scheduler.triggers (" +
							"\"FeedId\", \"RoutingKey\", \"VirtualHost\", \"Enabled\", \"RepeatInterval\", " +
							"\"CreatedAtUtc\", \"StartAtUtc\", \"UpdatedAtUtc\", \"FireTimeUtc\", " +
							"\"FireCount\", \"Payload\") " +
						"SELECT \"Id\", @RoutingKey, @VirtualHost, @Enabled, @RepeatInterval, " +
							"@CreatedAtUtc, @StartAtUtc, @UpdatedAtUtc, @FireTimeUtc, " +
							"0, @Payload " +
						"FROM scheduler.feeds " +
						"WHERE \"SourceId\"=@SourceId " +
						"ON CONFLICT ON CONSTRAINT ux_triggers_feed_id_routing_key " +
						"DO " +
							"UPDATE SET \"VirtualHost\"=EXCLUDED.\"VirtualHost\", " +
							"\"Enabled\"=EXCLUDED.\"Enabled\", " +
							"\"RepeatInterval\"=EXCLUDED.\"RepeatInterval\", " +
							"\"StartAtUtc\"=EXCLUDED.\"StartAtUtc\", " +
							"\"Payload\"=EXCLUDED.\"Payload\", " +
							"\"UpdatedAtUtc\"=EXCLUDED.\"UpdatedAtUtc\" " +
						"returning *;";

			return Connect(cx =>
			{
				return cx.Query<TriggerEntity>(
					query,
					new
					{
						SourceId = source.Id,
						entity.RoutingKey,
						entity.VirtualHost,
						entity.Enabled,
						entity.RepeatInterval,
						entity.CreatedAtUtc,
						entity.StartAtUtc,
						entity.UpdatedAtUtc,
						entity.FireTimeUtc,
						entity.Payload
					}
				).Select(x => x.FromEntity());
			});
		}

		Trigger ITriggersRepository.Find(long id)
		{
			return GetById(id)?.FromEntity();
		}

		public IEnumerable<Tuple<Trigger, Feed, Source>> Find(TriggerFilter filter)
		{
			var query = "SELECT s.*, t.*, f.* " +
						"FROM scheduler.triggers t " +
							$"INNER JOIN scheduler.feeds f on f.\"Id\" = t.\"FeedId\" {WhereFeed(filter)} " +
							$"INNER JOIN scheduler.sources s on s.\"Id\" = f.\"SourceId\" {WhereSource(filter)} " +
						$"{WhereTrigger(filter)} " +
						$"ORDER BY f.\"OriginalUrl\" LIMIT {filter.Count} OFFSET {filter.Offset};";
			return Connect(cx =>
			{
				return cx.Query<SourceEntity, TriggerEntity, FeedEntity, Tuple<Trigger, Feed, Source>>(
					query,
					(sourceEntity, triggerEntity, feedEntity) =>
					{
						if (sourceEntity == null || triggerEntity == null || feedEntity == null)
							return null;
						return new Tuple<Trigger, Feed, Source>(
							triggerEntity.FromEntity(),
							feedEntity.FromEntity(null),
							sourceEntity.FromEntity());
					})
					.Where(x => x != null);
			});
		}

		IEnumerable<Trigger> ITriggersRepository.FindTriggersToFire()
		{
			return Connect(cx =>
			{
				return cx
					.Query<TriggerEntity>("select * from scheduler.get_triggers_to_fire()")
					.Select(x => x.FromEntity());
			});
		}

		public Trigger Update(Trigger trigger)
		{
			var newEntity = TriggerEntity.ToEntity(trigger);
			return Connect(cx =>
			{
				return cx.Query<TriggerEntity>(
					"update scheduler.triggers " +
					"set \"RoutingKey\"=@RoutingKey, \"VirtualHost\"=@VirtualHost, \"Enabled\"=@Enabled,  \"RepeatInterval\"=@RepeatInterval, " +
					"\"Payload\"=@Payload, \"UpdatedAtUtc\"=@newUpdatedAtUtc, \"StartAtUtc\"=@StartAtUtc " +
					"where \"Id\"=@Id and \"UpdatedAtUtc\"=@UpdatedAtUtc " +
					"returning *;",
					new
					{
						newEntity.RoutingKey,
						newEntity.VirtualHost,
						newEntity.Enabled,
						newEntity.RepeatInterval,
						newEntity.Payload,
						newUpdatedAtUtc = DateTime.UtcNow,
						newEntity.StartAtUtc,
						newEntity.Id,
						newEntity.UpdatedAtUtc
					}).Single().FromEntity();
			});
		}

		public int Delete(Trigger trigger)
		{
			return Connect(cx =>
			{
				cx.Execute("delete from scheduler.triggers where \"Id\"=@Id;", trigger);
				return 1;
			});
		}

		private TriggerEntity GetById(long id)
		{
			return Connect(cx =>
			{
				return cx.Query<TriggerEntity>(
					"select * from scheduler.triggers where \"Id\"=@id", 
					new { id }).SingleOrDefault();
			});
		}

		private static string WhereFeed(TriggerFilter filter)
		{
			var result = "";
			if (!filter.FeedActualUrlPrefix.IsNullOrWhiteSpace())
			{
				result += $"AND LOWER(f.\"ActualUrl\") like '%{filter.FeedActualUrlPrefix.ToLower()}%' ";
			}
			else if (!filter.FeedOriginalUrlPrefix.IsNullOrWhiteSpace())
			{
				result += $"AND LOWER(f.\"OriginalUrl\") like '%{filter.FeedOriginalUrlPrefix.ToLower()}%' ";
			}

			if (filter.FeedType.HasValue)
			{
				result += $"AND f.\"FeedType\"='{filter.FeedType.Value:G}' ";
			}

			return result;
		}

		private static string WhereSource(TriggerFilter filter)
		{
			var result = "";
			if (!filter.SourceUrlPrefix.IsNullOrWhiteSpace())
			{
				result += $"AND LOWER(s.\"Url\") like '%{filter.SourceUrlPrefix.ToLower()}%' ";
			}
			if (filter.SourceType.HasValue)
			{
				result += $"AND s.\"SourceType\"='{filter.SourceType.Value:G}' ";
			}
			return result;
		}

		private static string WhereTrigger(TriggerFilter filter)
		{
			string result = "";
			if (!filter.RoutingKeyPrefix.IsNullOrWhiteSpace())
			{
				result += $"AND LOWER(t.\"RoutingKey\") like '{filter.RoutingKeyPrefix.ToLower()}%' ";
			}

			if (!filter.VirtualHostPrefix.IsNullOrWhiteSpace())
			{
				result += $"AND LOWER(t.\"VirtualHost\") like '{filter.VirtualHostPrefix.ToLower()}%' ";
			}
			if (filter.TriggersFilter.HasValue)
			{
				switch (filter.TriggersFilter)
				{
					case TriggersFilter.Disabled:
						result += "AND t.\"Enabled\" = false ";
						break;
					case TriggersFilter.Enabled:
						result += "AND t.\"Enabled\" = true ";
						break;
				}
			}

			if (!result.IsNullOrEmpty())
			{
				return $"WHERE {Regex.Replace(result, "^AND\\s", "")}";
			}
			return result;
		}
	}
}