using System;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.Proxy.Impl
{
	internal sealed class ProxyRepositorySettings: ConfigBase, IProxyRepositoryConfig
	{
		public ProxyRepositorySettings(XElement sectionXml) : base(sectionXml)
		{
		}
		public Uri ProxyListUri { get; private set; }
		public TimeSpan ResourcePoolTtl { get; private set; }
		public TimeSpan ResourceRemoveDelay { get; private set; }


		protected override void BuildConfig()
		{
			ProxyListUri = GetAttributeValueAndCast(
				nameof(ProxyListUri),
				(string value, out Uri result) => Uri.TryCreate(value, UriKind.Absolute, out result),
				"http://172.16.252.5:5195/api/v1/proxy/valid");

			ResourcePoolTtl = GetAttributeValueAndCast<TimeSpan>(nameof(ResourcePoolTtl), TimeSpan.TryParse, "00:20:00");
			ResourceRemoveDelay = GetAttributeValueAndCast<TimeSpan>(nameof(ResourceRemoveDelay), TimeSpan.TryParse, "00:00:05");
		}
	}
}