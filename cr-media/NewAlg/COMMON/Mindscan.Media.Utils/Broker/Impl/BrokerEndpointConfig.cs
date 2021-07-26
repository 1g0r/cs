using System;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal sealed class BrokerEndpointConfig : ConfigBase, IBrokerEndpointConfig
	{
		public BrokerEndpointConfig(XElement sectionXml) : base(sectionXml)
		{
		}

		public string Name { get; private set; }
		public string Host { get; private set; }
		public string UserName { get; private set; }
		public string Password { get; private set; }
		public string VirtualHost { get; private set; }
		public TimeSpan ConnectionTimeout { get; private set; }
		public TimeSpan RecoveryInterval { get; private set; }
		public TimeSpan RetryWaitInterval { get; private set; }
		public ushort PrefetchCount { get; private set; }
		public ushort ConsumersCount { get; private set; }
		public int MaxMessageSize { get; private set; }

		protected override void BuildConfig()
		{
			Name = GetAttributeValue(nameof(Name), "default");
			Host = GetAttributeValue(nameof(Host), "localhost");
			VirtualHost = GetAttributeValue(nameof(VirtualHost), "/");
			UserName = GetAttributeValue(nameof(UserName), false);
			Password = GetAttributeValue(nameof(Password), false);
			ConnectionTimeout = GetAttributeValueAndCast<TimeSpan>(nameof(ConnectionTimeout), TimeSpan.TryParse, "00:00:15");
			RecoveryInterval = GetAttributeValueAndCast<TimeSpan>(nameof(RecoveryInterval), TimeSpan.TryParse, "00:01:00");
			RetryWaitInterval = GetAttributeValueAndCast<TimeSpan>(nameof(RetryWaitInterval), TimeSpan.TryParse, "00:00:10");
			PrefetchCount = GetAttributeValueAndCast<ushort>(nameof(PrefetchCount), ushort.TryParse, "32");
			ConsumersCount = GetAttributeValueAndCast<ushort>(nameof(ConsumersCount), ushort.TryParse, "1");
			MaxMessageSize = GetAttributeValueAndCast<int>(nameof(MaxMessageSize), int.TryParse, "5242880");
		}
	}
}