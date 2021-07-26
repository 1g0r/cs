using System;

namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal sealed class DisposableValueStub : IDisposable
	{
		public int DisposeCount;
		public void Dispose()
		{
			DisposeCount++;
		}
	}
}