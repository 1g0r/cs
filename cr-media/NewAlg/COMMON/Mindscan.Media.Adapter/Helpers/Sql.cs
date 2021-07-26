namespace Mindscan.Media.Adapter.Helpers
{
	internal static class Sql
	{
		public static InsertExpressionBuilder<T> Insert<T>(string tableName, T entity) where T : class
		{
			return new InsertExpressionBuilder<T>(entity)
				.Insert(tableName);
		}

		public static FunctionExpressionBuilder<T> Function<T>(string name, T entity) where T : class
		{
			return new FunctionExpressionBuilder<T>(entity)
				.Name(name);
		}
	}
}
