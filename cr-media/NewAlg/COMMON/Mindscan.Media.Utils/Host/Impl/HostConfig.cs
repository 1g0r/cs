using System;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.Host.Impl
{
	internal class HostConfig : ConfigBase, IHostConfig
	{
		public HostConfig(XElement sectionXml) : base(sectionXml)
		{
		}

		protected override void BuildConfig()
		{
			StartTimeout = GetAttributeValueAndCast<TimeSpan>(nameof(StartTimeout), TimeSpan.TryParse, "00:00:30");
			StopTimeout = GetAttributeValueAndCast<TimeSpan>(nameof(StopTimeout), TimeSpan.TryParse, "00:00:30");
			Name = GetAttributeValue(nameof(Name));
			DisplayName = GetAttributeValue(nameof(DisplayName), false);
			Description = GetAttributeValue(nameof(Description), false);
			InstanceName = GetAttributeValue(nameof(InstanceName), false);
			User = GetAttributeValue(nameof(User), false);
			Password = GetAttributeValue(nameof(Password), false);
		}

		public TimeSpan StartTimeout { get; private set; }
		public TimeSpan StopTimeout { get; private set; }
		public string Name { get; private set; }
		public string DisplayName { get; private set; }
		public string Description { get; private set; }
		public string InstanceName { get; private set; }
		public string User { get; private set; }
		public string Password { get; private set; }
	}
}