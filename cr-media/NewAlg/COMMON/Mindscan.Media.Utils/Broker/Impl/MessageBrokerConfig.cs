using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal sealed class MessageBrokerConfig : ConfigBase, IMessageBrokerConfig
	{
		public MessageBrokerConfig(XElement sectionXml) : base(sectionXml)
		{
		}

		public Dictionary<string, IBrokerEndpointConfig> Endpoints { get; private set; }

		protected override void BuildConfig()
		{
			Endpoints = new Dictionary<string, IBrokerEndpointConfig>();
			foreach (var endpoint in Raw?.Elements("endpoint") ?? Enumerable.Empty<XElement>())
			{
				var endpointSettings = new BrokerEndpointConfig(endpoint);
				var key = string.IsNullOrWhiteSpace(endpointSettings.Name) ? "default" : endpointSettings.Name.ToLower();
				if (!Endpoints.ContainsKey(key))
				{
					Endpoints.Add(key, endpointSettings);
				}
			}
		}
	}
}