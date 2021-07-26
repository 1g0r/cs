using System;
using System.Collections.Generic;

namespace Mindscan.Media.Utils.DbConnector
{
	public interface ISqlExecutor
	{
		IEnumerable<T> List<T>(string sql, object param = null);
		IEnumerable<T> List<T>(string sql, Func<IList<T>, bool> commit, object param = null);
		T Get<T>(string sql, object param = null, bool readUncommitted = false);
		T Get<T>(string sql, Func<T, bool> commit, object param = null);
		void Execute(string sql, object param = null);
	}
}