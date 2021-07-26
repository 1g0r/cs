using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Core.Commands;

namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal class DisposableCommandStub : CommandBase<DisposableValueStub>
	{
		public readonly List<DisposableValueStub> Disposables = new List<DisposableValueStub>();
		protected internal override void FromExpression(string json)
		{

		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, DisposableValueStub data)
		{
			var result = new DisposableValueStub();
			Disposables.Add(result);
			return result;
		}
	}
}
