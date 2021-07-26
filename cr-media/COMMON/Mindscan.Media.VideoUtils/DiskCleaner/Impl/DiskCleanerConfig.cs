using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.VideoUtils.DiskCleaner.Impl
{
	internal sealed class DiskCleanerConfig: ConfigBase, IDiskCleanerConfig
	{
		public DiskCleanerConfig(XElement sectionXml) : base(sectionXml)
		{
		}

		protected override void BuildConfig()
		{
			RootDirectory = GetAttributeValue(nameof(RootDirectory));
			HoursOfDeleteDelay = GetAttributeValueAndCast<int>(nameof(HoursOfDeleteDelay), int.TryParse, "5");
			HoursOfExceptionDelay = GetAttributeValueAndCast<int>(nameof(HoursOfExceptionDelay), int.TryParse, "1");
			GBytesToKeepOnRoot = GetAttributeValueAndCast<int>(nameof(GBytesToKeepOnRoot), int.TryParse, "15");
		}

		public string RootDirectory { get; private set; }
		public int HoursOfDeleteDelay { get; private set; }
		public int HoursOfExceptionDelay { get; private set; }
		public int GBytesToKeepOnRoot { get; private set; }
	}
}