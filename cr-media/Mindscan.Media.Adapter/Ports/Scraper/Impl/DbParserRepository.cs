using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Ports.Scheduler.Entities;
using Mindscan.Media.Adapter.Ports.Scraper.Entities;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Scraper.Impl
{
	internal sealed class DbParserRepository : PortBase, IParsersRepository
	{
		public DbParserRepository(ILoggerFactory factory, IRepositoryConfig config)
			: base(factory, config)
		{
		}

		public Parser Add(Parser parser)
		{
			var sql = "insert into scraper.parsers (" +
						"\"SourceId\", " +
						"\"CreatedAtUtc\", " +
						"\"UpdatedAtUtc\", " +
						"\"Host\", " +
						"\"Path\", " +
						"\"Encoding\", " +
						"\"UseBrowser\", " +
						"\"AdditionalInfo\", " +
						"\"Tag\")" +
					"values(@SourceId, @CreatedAtUtc, @UpdatedAtUtc, @Host, @Path, @Encoding, @UseBrowser, @AdditionalInfo, @Tag)" +
					"returning *;";

			var entity = ParserEntity.ToEntity(parser);
			entity.CreatedAtUtc = entity.UpdatedAtUtc = DateTime.UtcNow;
			return Connect(cx =>
			{
				return cx.Query<ParserEntity>(sql, entity)
					.Single()
					.FromEntity();
			});
		}

		public IEnumerable<Tuple<Parser, Source>> Find(NormalizedUrl pageUrl, string tag)
		{
			var query = "SELECT p.*, j.*, s.* " +
						"FROM scraper.parsers p " +
						"LEFT JOIN scraper.jsons j on j.\"Id\"=p.\"Id\" and j.\"Enabled\"=true " +
						"INNER JOIN scheduler.sources s on s.\"Id\"=p.\"SourceId\" " +
						"WHERE @Host like '%'||\"Host\" AND \"Tag\" is not distinct from @Tag " +
						"ORDER BY CHAR_LENGTH(p.\"Host\") desc;";
			return Connect(cx =>
			{
				return cx
					.Query<ParserEntity, ParserJsonEntity, SourceEntity, Tuple<Parser, Source>>(
						query,
						(parser, json, source) =>
						{
							if (parser != null && source != null)
							{
								parser.Json = json;
								return new Tuple<Parser, Source>(parser.FromEntity(), source.FromEntity());
							}
							return null;
						},
						new { pageUrl.Host, Tag = tag })
					.Where(x => x != null );
			});
		}

		public Parser Find(long id)
		{
			return GetById(id)?.FromEntity();
		}

		public string FindJson(long id)
		{
			return Connect(cx =>
			{
				var entity = cx.Query<ParserJsonEntity>(
					"select * from scraper.jsons where \"Id\"=@id and \"Enabled\"=true;",
					new { id }
				).SingleOrDefault();

				return entity?.Value;
			});
		}

		public IEnumerable<Parser> List(long sourceId)
		{
			var sql = "SELECT p.*, t.*, j.* " +
					"FROM scraper.parsers p " +
						"LEFT JOIN scraper.tests t on t.\"ParserId\"=p.\"Id\" " +
						"LEFT JOIN scraper.jsons j on j.\"Id\"=p.\"Id\" AND j.\"Enabled\"=true " +
					"WHERE p.\"SourceId\"=@sourceId " +
					"ORDER BY p.\"Host\", p.\"Path\" nulls first;";

			var parsers = new Dictionary<long, Parser>();

			var result = Connect(cx =>
			{

				return cx.Query<ParserEntity, ParserTestEntity, ParserJsonEntity, Parser>(
					sql,
					(parserEntity, testEntity, jsonEntity) =>
					{
						Parser parser;
						if (!parsers.TryGetValue(parserEntity.Id, out parser))
						{
							parserEntity.Json = jsonEntity;
							parser = parserEntity.FromEntity();
							parsers.Add(parserEntity.Id, parser);
						}

						if (testEntity != null)
						{
							parser.Tests.Add(testEntity.FromEntity());
						}

						return parser;
					},
					new { sourceId }
				).Distinct();
			});
			return result;
		}

		public Parser Update(Parser parser)
		{
			var newEntity = ParserEntity.ToEntity(parser);
			return Connect((cx, tx) =>
			{
				var result = cx.Query<ParserEntity>(
					"select * from scraper.parser_update(" +
					"@Id, @UpdatedAtUtc, @Host, @Path, @AdditionalInfo, @Tag, @Encoding, @UseBrowser);",
					newEntity,
					tx
				).SingleOrDefault();

				if (result != null && !newEntity.Json.Value.IsNullOrWhiteSpace())
				{
					result.Json = cx.Query<ParserJsonEntity>(
						"select * from scraper.parser_json_update(@UpdatedAtUtc, @Id, @Value);",
						new
						{
							result.UpdatedAtUtc,
							newEntity.Json.Id,
							newEntity.Json.Value
						},
						tx
					).SingleOrDefault();
				}

				return result?.FromEntity();
			});

		}

		public Parser Update(Parser parser, string json)
		{
			var newEntity = ParserEntity.ToEntity(parser);
			return Connect(cx =>
			{
				var result = cx.Query<ParserJsonEntity>(
					"select * from scraper.parser_json_update(@UpdatedAtUtc, @Id, @Value);",
					new
					{
						newEntity.UpdatedAtUtc,
						newEntity.Id,
						Value = json
					}
				).SingleOrDefault();

				newEntity.Json = result;

				return newEntity.FromEntity();
			});
		}

		public int Delete(Parser parser)
		{
			return Connect(cx =>
			{
				cx.Execute(
					"delete from scraper.parsers where \"Id\"=@Id and \"UpdatedAtUtc\"=@UpdatedAtUtc;", 
					parser);
				return 1;
			});
		}

		private ParserEntity GetById(long id)
		{
			var sql = "select p.*, j.* " +
					"from scraper.parsers p " +
					"left join scraper.jsons j on j.\"Id\" = p.\"Id\" and j.\"Enabled\" = true " +
					"where p.\"Id\"=@id;";
			return Connect(cx =>
			{
				return cx.Query<ParserEntity, ParserJsonEntity, ParserEntity>(
					sql,
					(parser, json) =>
					{
						parser.Json = json;
						return parser;
					},
					new { id }).SingleOrDefault();
			});
		}
	}
}