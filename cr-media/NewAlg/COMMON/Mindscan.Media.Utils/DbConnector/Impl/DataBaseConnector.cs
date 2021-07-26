using System;
using System.Data.SqlClient;
using Npgsql;

namespace Mindscan.Media.Utils.DbConnector.Impl
{
	internal class DataBaseConnector : IDataBaseConnector
	{
		public ISqlExecutor NpgSql(string connection)
		{
			if (string.IsNullOrEmpty(connection))
				throw new ArgumentNullException(nameof(connection));

			return new SqlExecutor<NpgsqlConnection>(() => new NpgsqlConnection(connection));
		}

		public ISqlExecutor MsSql(string connection)
		{
			if (string.IsNullOrEmpty(connection))
				throw new ArgumentNullException(nameof(connection));

			return new SqlExecutor<SqlConnection>(() => new SqlConnection(connection));
		}
	}
}