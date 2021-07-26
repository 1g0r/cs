using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;

namespace Mindscan.Media.Utils.DbConnector.Impl
{
	internal class SqlExecutor<TConnection> : ISqlExecutor where TConnection : DbConnection
	{
		private readonly Func<TConnection> _connectionFactory;
		public SqlExecutor(Func<TConnection> connectionFactory)
		{
			SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
			SqlMapper.AddTypeMap(typeof(long), DbType.Int64);
			_connectionFactory = connectionFactory;
		}
		public IEnumerable<T> List<T>(string sql, object param = null)
		{
			return Execute(c => c.Query<T>(sql, param, commandType: CommandType.Text));
		}

		public IEnumerable<T> List<T>(string sql, Func<IList<T>, bool> commit, object param = null)
		{
			return ExecuteWithTransaction(
				(con, tran) => con.Query<T>(sql, param, tran, commandType: CommandType.Text).ToList(), 
				commit, 
				IsolationLevel.ReadCommitted);
		}

		public T Get<T>(string sql, object param = null, bool readUncommitted = false)
		{
			if (readUncommitted)
			{
				return ExecuteWithTransaction(
					(c, t) =>  c.Query<T>(sql, param, t, commandType: CommandType.Text).SingleOrDefault(), 
					null, 
					IsolationLevel.ReadUncommitted);
			}
			return Execute(c => c.Query<T>(sql, param, commandType: CommandType.Text).SingleOrDefault());
		}

		public T Get<T>(string sql, Func<T, bool> commit, object param = null)
		{
			return ExecuteWithTransaction(
				(c, t) => c.Query<T>(sql, param, t, commandType: CommandType.Text).SingleOrDefault(),
				commit, 
				IsolationLevel.ReadCommitted);
		}

		public void Execute(string sql, object param = null)
		{
			Execute(c => c.Execute(sql, param));
		}

		private T Execute<T>(Func<TConnection, T> func)
		{
			using (var connection = _connectionFactory())
			{
				connection.Open();
				return func(connection);
			}
		}

		private T ExecuteWithTransaction<T>(Func<TConnection, DbTransaction, T> sqlFunc, Func<T, bool> commitPredicate, IsolationLevel isolation)
		{
			using (var connection = _connectionFactory())
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction(isolation))
				{
					var result = sqlFunc(connection, transaction);
					if (commitPredicate == null || commitPredicate(result))
					{
						transaction.Commit();
						return result;
					}
					transaction.Rollback();
					return default(T);
				}
				
			}
		}
	}
}