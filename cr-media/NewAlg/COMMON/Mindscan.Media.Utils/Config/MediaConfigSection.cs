using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Mindscan.Media.Utils.Config
{
	public sealed class MediaConfigSection : ConfigurationSection
	{
		public XElement SectionXml { get; private set; }

		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			SectionXml = XElement.Load(reader);
		}
	}
}