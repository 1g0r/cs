using System;
using System.Threading;

namespace Mindscan.Media.Utils.Broker
{
	public interface IMessageBrokerBuilder : IDisposable
	{
		IMessageBroker Build(CancellationToken token, string endpointName = "default");
	}
}