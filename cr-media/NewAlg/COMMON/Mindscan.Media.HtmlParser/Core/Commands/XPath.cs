using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Sgml;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.XPath)]
	internal class XPath : CommandBase<string>
	{
		protected internal override void FromExpression(string json)
		{

		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, string html)
		{
			if (string.IsNullOrWhiteSpace(html))
				return null;

			using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(html)))
			using (var decoder = new MnemonicDecoder(reader))
			using (var sgml = new SgmlReader())
			{
				sgml.DocType = "HTML";
				sgml.CaseFolding = CaseFolding.ToLower;
				sgml.InputStream = decoder;
				sgml.IgnoreDtd = true;

				var doc = XDocument.Load(sgml);
				StripNamespace(doc);
				return doc;
			}
		}

		static void StripNamespace(XDocument document)
		{
			if (document.Root == null)
			{
				return;
			}
			foreach (var element in document.Root.DescendantsAndSelf())
			{
				element.Name = element.Name.LocalName;
				element.ReplaceAttributes(GetAttributes(element));
			}
		}

		static IEnumerable GetAttributes(XElement xElement)
		{
			return xElement.Attributes()
				.Where(x => !x.IsNamespaceDeclaration)
				.Select(x => new XAttribute(x.Name.LocalName, x.Value));
		}
	}
}