using System.Collections.Generic;

namespace Mindscan.Media.HtmlParser
{
	public interface IPipelineSupportedCommands
	{
		IPipelineCommand Const(params string[] values);
		IPipelineCommand Css();
		IPipelineCommand XPath();
		IPipelineCommand Join(string delimiter = null);
		IPipelineCommand Remove(params string[] patterns);
		IPipelineCommand Skip(params string[] patterns);
		IPipelineCommand Take(int count = 1);
		IPipelineCommand TakeFirst(params string[] patterns);
		IPipelineCommand TakeWhile(params string[] patterns);
		IPipelineCommand BreakWhen(params string[] patterns);
		IPipelineCommand Node(params string[] patterns);
		IPipelineCommand Nodes(params string[] patterns);
		IPipelineCommand Text(params string[] skipPatterns);
		IPipelineCommand TextLines(IEnumerable<string> skipPatterns = null, IEnumerable<string> newLinePatterns = null);
		IPipelineCommand Attr(params string[] attributeNames);
		IPipelineCommand SkipLast(int count = 1);
		IPipelineCommand ToDate(string cultureName, params string[] formats);
		IPipelineCommand ToInt();
		IPipelineCommand Replace(string oldValue, string newValue = null);
		IPipelineCommand Next(params string[] patterns);
		IPipelineCommand Distinct();
		IPipelineCommand Attrs(params string[] attributeNames);
		IPipelineCommand Format(string pattern);
		IPipelineCommand Match(string[] patterns, string groupName = "");
		IPipelineCommand Matches(string[] patterns, string groupName = "");
		IPipelineCommand Json();
		IPipelineCommand TakeLast(params string[] patterns);
		IPipelineCommand SkipWithText(IEnumerable<string> skipPatterns = null, IEnumerable<string> textPatterns = null);
		IPipelineCommand TakeWithText(IEnumerable<string> skipPatterns = null, IEnumerable<string> textPatterns = null);
		IPipelineCommand Unescape();
		IPipelineCommand ToFakeUrl();
		IPipelineCommand Decode();
		IPipelineCommand Nop();
		IPipelineCommand Split(string pattern);
		IPipelineCommand ToUrl();
	}
}