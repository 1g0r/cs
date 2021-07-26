using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace Mindscan.Media.Utils.Broker.Temp
{
	public abstract class MassTransitEnvelop
	{
		[JsonProperty("messageId")]
		public Guid MessageId { get; set; }
		[JsonProperty("conversationId")]
		public Guid ConversationId { get; set; }
		[JsonProperty("correlationId")]
		public Guid CorrelationId { get; set; }
		[JsonProperty("sourceAddress")]
		public string SourceAddress { get; set; }
		[JsonProperty("destinationAddress")]
		public string DestinationAddress { get; set; }
		[JsonProperty("messageType")]
		public List<string> MessageType { get; set; }
		[JsonProperty("headers")]
		public Dictionary<string, object> Headers { get; set; }
		[JsonProperty("host")]
		public MtHost Host { get; set; }

		public static MassTransitEnvelop<T> Map<T>(T message, MassTransitEnvelop envelop = null)
		{
			var type = typeof(T);
			var result = new MassTransitEnvelop<T>(envelop)
			{
				Message = message,
				MessageType = new List<string>
				{
					$"urn:message:{type.Namespace}:{type.Name}"
				}
			};
			if (type.BaseType != null)
			{
				result.MessageType.Add($"urn:message:{type.BaseType.Namespace}:{type.BaseType.Name}");
			}

			return result;
		}
	}
	public class MassTransitEnvelop<TMessage> : MassTransitEnvelop
	{
		[JsonProperty("message")]
		public TMessage Message { get; set; }

		public MassTransitEnvelop(MassTransitEnvelop envelop)
		{
			if (envelop != null)
			{
				MessageId = envelop.MessageId;
				ConversationId = envelop.ConversationId;
				CorrelationId = envelop.CorrelationId;
				Headers = envelop.Headers;
			}
			Host = GetHost();
		}
		private MtHost GetHost()
		{
			var process = Process.GetCurrentProcess();
			var assembly = Assembly.GetEntryAssembly().GetName();
			return new MtHost
			{
				Assembly = assembly.Name,
				AssemblyVersion = assembly.Version.ToString(3),
				ProcessName = process.ProcessName,
				ProcessId = process.Id,
				MachineName = Environment.MachineName,
				FrameworkVersion = Environment.Version.ToString(),
				OperatingSystemVersion = Environment.OSVersion.VersionString,
				
			};
		}
	}

	
}
