using System;

namespace Mindscan.Media.Utils.Broker
{
	public class MessageSizeException<TMessage> : Exception
	{
		public MessageSizeException(string exchange, string routingKey, int actualSize, int maxSize)
			:base($"Unable to send message to exchange '{exchange}' with routing key '{routingKey}' because message body size {actualSize} exceeds max body size {maxSize}.")
		{
			
		}
	}
}