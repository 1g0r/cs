namespace Mindscan.Media.Utils.Broker
{
	public static class MessageBrokerHeaders
	{
		internal const string Type = "Broker-MessageType";
		internal const string ErrorMessage = "Broker-Error";
		internal const string ProcessName = "Broker-ProcessName";
		internal const string ProcessId = "Broker-ProcessId";
		internal const string MachineName = "Broker-MachineName";
		internal const string Timestamp = "Broker-Timestamp";
		public static string MessageTtlMilliseconds => "Broker-MessageTtl";
	}
}