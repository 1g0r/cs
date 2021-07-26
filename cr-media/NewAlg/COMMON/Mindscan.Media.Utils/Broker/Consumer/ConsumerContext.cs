using System;
using System.Text;
using System.Threading;
using Mindscan.Media.Utils.Broker.Temp;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Mindscan.Media.Utils.Broker.Consumer
{
	internal class ConsumerContext
	{
		private readonly BasicDeliverEventArgs _args;
		internal readonly MassTransitEnvelop MtEnvelop;
		private object _message;
		internal ConsumerContext(BasicDeliverEventArgs args, string queue, MassTransitEnvelop envelop, CancellationToken token)
		{
			_args = args;
			MtEnvelop = envelop;
			Token = token;
			Queue = queue;
		}

		public T Message<T>()
		{
			if (_message == null)
			{
				if (!TryDeserializeMassTransitMessage<T>(_args.Body, out _message))
				{
					_message = Deserialize<T>(_args.Body);
				}
			}
			return (T) _message;
		}

		public CancellationToken Token { get; }
		public string Queue { get; }

		public static byte[] Serialize<T>(T message)
		{
			var str = JsonConvert.SerializeObject(message, new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore
			});
			return Encoding.UTF8.GetBytes(str);
		}

		private static T Deserialize<T>(byte[] body)
		{
			if (body == null || body.Length == 0)
				return default(T);
			return JsonConvert.DeserializeObject<T>(
				Encoding.UTF8.GetString(body));
		}

		private static bool TryDeserializeMassTransitMessage<T>(byte[] body, out object message)
		{
			try
			{
				var mt = Deserialize<MassTransitEnvelop<T>>(body);
				message = mt.Message;
				return message != null;
			}
			catch (Exception ex)
			{
				message = default(T);
				return false;
			}
		}
	}

	public sealed class ConsumerContext<T>
	{
		private readonly ConsumerContext _rawContext;
		public readonly MassTransitEnvelop MtEnvelop;
		internal ConsumerContext(ConsumerContext rawContext)
		{
			_rawContext = rawContext;
			Message =_rawContext.Message<T>();
			MtEnvelop = _rawContext.MtEnvelop;
		}

		public T Message { get; private set; }
		public CancellationToken Token => _rawContext.Token;
		public string Queue => _rawContext.Queue;
	}
}