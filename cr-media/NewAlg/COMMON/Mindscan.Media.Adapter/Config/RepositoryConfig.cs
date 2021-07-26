using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Adapter.Config
{
	internal sealed class RepositoryConfig : ConfigBase, IRepositoryConfig
	{
		public RepositoryConfig(string sectionName) : base(sectionName)
		{
		}

		protected override void BuildConfig()
		{
			ConnectionString = GetAttributeValue(nameof(ConnectionString));
		}

		public string ConnectionString { get; private set; }
	}
}