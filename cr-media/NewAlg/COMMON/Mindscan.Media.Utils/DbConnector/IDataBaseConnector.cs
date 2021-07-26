namespace Mindscan.Media.Utils.DbConnector
{
	public interface IDataBaseConnector
	{
		ISqlExecutor NpgSql(string connection);
		ISqlExecutor MsSql(string connection);
	}
}
