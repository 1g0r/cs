using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Castle.Core.Internal;
using Mindscan.Media.Utils.Broker.Consumer;
using Mindscan.Media.Utils.Logger;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal class SenderChannel: ChannelBase
	{
		public SenderChannel(string exchange, string routingKey, BrokerConnection connection, CancellationToken token, ILoggerFactory factory)
			:base(exchange, routingKey, connection, token, factory)
		{
		}


		public void Send<TMessage>(TMessage message, Dictionary<string, string> customHeaders)
		{
			var props = Call(m => m.CreateBasicProperties());
			if (props == null)
				return;
			props.ContentType = "application/json";
			props.Persistent = true;
			props.ContentEncoding = "utf-8";
			props.Headers = GetHeaders<TMessage>(customHeaders);
			var expiration = GetExpiration(customHeaders);
			if (expiration != null)
			{
				props.Expiration = expiration;
			}
			var body = ConsumerContext.Serialize(message);
			if (body.Length > Connection.Config.MaxMessageSize)
			{
				throw new MessageSizeException<TMessage>(Exchange, RoutingKey, body.Length, Connection.Config.MaxMessageSize);
			}

			Call(m =>
			{
				if (string.IsNullOrEmpty(RoutingKey))
				{
					Logger.DebugFormat(LogMessages.SendMessageNoRoutingKey, Exchange);
				}
				else
				{
					Logger.DebugFormat(LogMessages.SendMessage, Exchange, RoutingKey);
				}
				m.BasicPublish(Exchange, RoutingKey, props, body);
			});
		}

		public void SendError(string queue, BasicDeliverEventArgs args, Exception ex)
		{
			if (!Connection.IsAlive)
				return;

			CreateQueue(queue, 1);
			SetErrorHeaders(args, ex);
			Call(m =>
			{
				if (string.IsNullOrEmpty(RoutingKey))
				{
					Logger.DebugFormat(LogMessages.SendMessageNoRoutingKey, Exchange);
				}
				else
				{
					Logger.DebugFormat(LogMessages.SendMessage, Exchange, RoutingKey);
				}
				m.BasicPublish(Exchange, RoutingKey, args.BasicProperties, args.Body);
			});
		}

		protected override void CustomConfigure()
		{
			CreateExchange(true, 1);
		}

		private Dictionary<string, object> GetHeaders<TMessage>(Dictionary<string, string> customHeaders)
		{
			var process = Process.GetCurrentProcess();
			var result = new Dictionary<string, object>
			{
				{ MessageBrokerHeaders.Type,  typeof(TMessage).FullName },
				{ MessageBrokerHeaders.MachineName,  Environment.MachineName },
				{ MessageBrokerHeaders.ProcessName,  process.ProcessName },
				{ MessageBrokerHeaders.ProcessId,  process.Id },
				{ MessageBrokerHeaders.Timestamp, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }
			};
			if (customHeaders != null)
			{
				foreach (var customHeader in customHeaders)
				{
					result.Add(customHeader.Key, customHeader.Value);
				}
			}

			return result;
		}

		private void SetErrorHeaders(BasicDeliverEventArgs args, Exception ex)
		{
			var headers = args.BasicProperties?.Headers;
			if (headers != null)
			{
				SetHeaderValue(headers, MessageBrokerHeaders.ErrorMessage, ex.ToString());
				var process = Process.GetCurrentProcess();
				SetHeaderValue(headers, MessageBrokerHeaders.MachineName, Environment.MachineName);
				SetHeaderValue(headers, MessageBrokerHeaders.ProcessName, process.ProcessName);
				SetHeaderValue(headers, MessageBrokerHeaders.ProcessId, process.Id.ToString());
				SetHeaderValue(headers, MessageBrokerHeaders.Timestamp, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
			}
		}

		private void SetHeaderValue(IDictionary<string, object> headers, string headerKey, string headerValue)
		{
			if (!headerValue.IsNullOrEmpty() && headerValue.Length > 5000)
			{
				headerValue = headerValue.Substring(0, 5000) + "...";
			}

			if (headers.ContainsKey(headerKey))
			{
				headers[headerKey] = headerValue;
			}
			else
			{
				headers.Add(headerKey, headerValue);
			}
		}

		private string GetExpiration(Dictionary<string, string> customHeaders)
		{
			if (customHeaders != null && customHeaders.ContainsKey(MessageBrokerHeaders.MessageTtlMilliseconds))
			{
				return customHeaders[MessageBrokerHeaders.MessageTtlMilliseconds];
			}
			return null;
		}
	}
}