using Mindscan.Media.Domain.Entities;

namespace Mindscan.Media.UseCase.Ports
{
	public interface IParserProcessor
	{
		string Validate(string json);
		string Execute(string json, string content, NormalizedUrl pageUrl, bool debug);
	}
}