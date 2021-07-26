using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests
{
	[TestFixture]
	public abstract class ParserTestsBase
	{
		private static readonly string HtmlPageTemplate = "<!DOCTYPE html><html><head><title>The Title</title></head><body>{0}</body></html>";
		public const string BaseUrl = "http://tests.com";
		public IPipelineSupportedCommands SupportedCommands;

		[OneTimeSetUp]
		public void SetUpOnes()
		{
			SupportedCommands = new PipelineSupportedCommands();
		}

		public static IDocument GetDocument(string htmlBody)
		{
			var html = FormatHtml(htmlBody);
			return BrowsingContext.New(Configuration.Default.WithLocaleBasedEncoding())
				.OpenAsync(r => r.Header("Content-Type", "text/html; charset=UTF-8;").Address(BaseUrl).Content(html)).Result;
		}

		public static string FormatHtml(string htmlBody)
		{
			return string.Format(HtmlPageTemplate, htmlBody);
		}

		protected List<TResult> GetImplementations<TResult>() where TResult : class
		{
			var type = typeof(TResult);
			return type.Assembly.GetTypes()
				.Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
				.Select(x => Activator.CreateInstance(x) as TResult)
				.ToList();
		}

		protected ParserContext GetContext(string url = "http://tests.com")
		{
			return new ParserContext(new Uri(url), false);
		}
	}
}
