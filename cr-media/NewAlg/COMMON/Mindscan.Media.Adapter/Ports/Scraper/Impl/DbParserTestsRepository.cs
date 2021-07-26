using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Helpers;
using Mindscan.Media.Adapter.Ports.Scraper.Entities;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.Ports.Scraper.Impl
{
	internal sealed class DbParserTestsRepository : PortBase, IParserTestsRepository
	{
		private readonly IParsersRepository _parserRepository;
		public DbParserTestsRepository(ILoggerFactory factory, IRepositoryConfig config, IParsersRepository parserRepository) 
			: base(factory, config)
		{
			_parserRepository = parserRepository;
		}

		public ParserTest Add(long parserId, ParserTest test)
		{
			/*if (test == null || test.ResultJson.IsNullOrWhiteSpace())
			{
				return null;
			}

			var parser = _parserRepository.Get(parserId);
			if (parser == null)
				throw new EntityNoFoundException(nameof(ParserEntity), parserId);

			test.CreatedAtUtc = test.UpdatedAtUtc = DateTime.UtcNow;
			test.ParserId = parserId;*/
			var entity = ParserTestEntity.ToEntity(test);

			return Connect(cx =>
			{
				return cx.Query<ParserTestEntity>(
					"insert into scraper.tests (\"ParserId\", \"CreatedAtUtc\", \"UpdatedAtUtc\", \"PageUrlPrefix\", \"PageUrl\", \"ResultJson\") " +
					"values(@ParserId, @CreatedAtUtc, @UpdatedAtUtc, @PageUrlPrefix, @PageUrl, @ResultJson) " +
					"returning *;", 
					entity
				).Single().FromEntity();
			});
		}

		public IEnumerable<ParserTest> List(long parserId)
		{
			throw new NotImplementedException();
		}

		public ParserTest Get(NormalizedUrl url)
		{
			if (url == null)
				return null;

			return Connect(cx =>
			{
				return cx.Query<ParserTestEntity>(
					"select * from scraper.tests where \"UrlHash\"=@Hash;",
					new { Hash = url.ComputeMd5Hash() } 
				).SingleOrDefault()?.FromEntity();
			});
		}

		public ParserTest Get(long id)
		{
			return Connect(cx =>
			{
				return cx.Query<ParserTestEntity>(
					"select * from scraper.tests where \"Id\"=@id",
					new { id }
				).SingleOrDefault()?.FromEntity();
			});
		}

		public ParserTest Update(long id, DateTime updatedAtUtc, ParserTest test)
		{
			throw new NotImplementedException();
		}

		public int Delete(int id, DateTime updatedAtUtc)
		{
			throw new NotImplementedException();
		}
	}
}