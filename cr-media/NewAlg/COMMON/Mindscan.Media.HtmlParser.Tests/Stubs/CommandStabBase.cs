using Mindscan.Media.HtmlParser.Core.Commands;

namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal abstract class CommandStabBase : CommandBase
	{
		protected internal override void FromExpression(string json)
		{
			throw new System.NotImplementedException();
		}

		protected internal override string BuildParametersJson()
		{
			throw new System.NotImplementedException();
		}

		internal int ExecuteCount;
	}
}