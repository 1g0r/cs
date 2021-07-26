using System;
using AngleSharp;
using AngleSharp.Dom;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Css)]
	internal class Css : CommandBase<string>
	{
		private readonly IBrowsingContext _browsingContext = BrowsingContext.New(Configuration.Default.WithLocaleBasedEncoding());

		protected internal override void FromExpression(string json)
		{
			
		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, string html)
		{
			if (IsValidHtml(html))
			{
				return GetDocument(html, context.PageUrl);
			}	
			return null;
		}

		IDocument GetDocument(string html, Uri pageUrl)
		{
			return _browsingContext.OpenAsync(x => x
				.Header("Content-Type", "text/html; charset=UTF-8;")
				.Content(html)
				.Address(pageUrl)
			).Result;
		}

		bool IsValidHtml(string html)
		{
			if (!string.IsNullOrWhiteSpace(html))
			{
				return HasTitleTag(html) ||
				       HasOpenHtmlTag(html) ||
				       HasClosingHtmlTag(html);
			}
			return false;
		}

		bool HasTitleTag(string html)
		{
			return html.IndexOf("<title", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		bool HasOpenHtmlTag(string html)
		{
			return html.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		bool HasClosingHtmlTag(string html)
		{
			return html.IndexOf("</html", StringComparison.InvariantCultureIgnoreCase) >= 0;
		}
	}
}