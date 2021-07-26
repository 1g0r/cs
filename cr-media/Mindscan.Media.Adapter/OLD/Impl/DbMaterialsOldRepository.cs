using System;
using System.Linq;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.OLD.Entities;
using Mindscan.Media.Adapter.Ports;
using Mindscan.Media.UseCase.OLD;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Adapter.OLD.Impl
{
	internal class DbMaterialsOldRepository: PortBase, IMaterialsRepository
	{
		public DbMaterialsOldRepository(ILoggerFactory factory, IRepositoryConfig config) 
			:base(factory, config)
		{
		}
		public DateTimeOffset? GetPublicationDate(Uri sourceUrl)
		{
			return Connect(cx =>
			{
				var sql = "SELECT published_at as PublicationDate, url as Url, source as Source " +
						"FROM integration.last_by_published_at " +
						"where source=@host;";

				return cx.Query<PublicationEntity>(
					sql,
					new {host = NormalizeHost(sourceUrl) }
				).SingleOrDefault();
			})?.PublicationDate;
		}

		public DateTimeOffset? UpdatePublicationDate(Uri sourceUrl, DateTimeOffset date, Uri pageUrl)
		{
			var result = Connect((cx, tx) =>
			{
				return cx.Query<PublicationEntity>(
					"SELECT * from integration.update_publication_date(@sourceUrl, @date, @pageUrl)",
					new
					{
						sourceUrl = NormalizeHost(sourceUrl),
						date,
						pageUrl = NormalizeUrl(pageUrl)
					},
					tx
				).SingleOrDefault();
			});

			return result?.PublicationDate;
		}


		private static string NormalizeHost(Uri url)
		{
			return $"http://{url.Normalize().Host}";
		}

		private static string NormalizeUrl(Uri url)
		{
			return url.Normalize().ToString();
		}
	}
}
