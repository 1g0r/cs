using System;
using System.Data;
using Dapper;
using Mindscan.Media.Adapter.Config;
using Mindscan.Media.Adapter.Helpers;
using Mindscan.Media.Utils.Logger;
using Npgsql;

namespace Mindscan.Media.Adapter.Ports
{
	internal abstract class PortBase
	{
		protected readonly ILogger Logger;
		private readonly string _connectionString;

		protected PortBase(ILoggerFactory factory, IRepositoryConfig config)
		{
			Logger = factory.CreateLogger(GetType().Name);
			_connectionString = config.ConnectionString;
			SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
			SqlMapper.AddTypeMap(typeof(long), DbType.Int64);
			SqlMapper.AddTypeHandler(new DateTimeHandler());
		}

		protected TResult Connect<TResult>(Func<NpgsqlConnection, TResult> func)
		{
			using (var connection = new NpgsqlConnection(_connectionString))
			{
				connection.Open();
				return func(connection);
			}
		}

		protected TResult Connect<TResult>(Func<NpgsqlConnection, NpgsqlTransaction, TResult> func)
		{
			using (var connection = new NpgsqlConnection(_connectionString))
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
				{
					try
					{
						var result = func(connection, transaction);
						transaction.Commit();
						return result;
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}
	}
}
