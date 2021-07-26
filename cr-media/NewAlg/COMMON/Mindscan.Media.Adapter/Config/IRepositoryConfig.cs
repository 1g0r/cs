using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Adapter.Config
{
	public interface IRepositoryConfig : IConfig
	{
		string ConnectionString { get; }
	}
}
