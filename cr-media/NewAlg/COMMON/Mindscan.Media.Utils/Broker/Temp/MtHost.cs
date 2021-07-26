using Newtonsoft.Json;

namespace Mindscan.Media.Utils.Broker.Temp
{
	public class MtHost
	{
		[JsonProperty("machineName")]
		public string MachineName { get; set;}
		[JsonProperty("processName")]
		public string ProcessName { get; set; }
		[JsonProperty("processId")]
		public int ProcessId { get; set; }
		[JsonProperty("assembly")]
		public string Assembly { get; set; }
		[JsonProperty("assemblyVersion")]
		public string AssemblyVersion { get; set; }
		[JsonProperty("frameworkVersion")]
		public string FrameworkVersion { get; set; }
		[JsonProperty("operatingSystemVersion")]
		public string OperatingSystemVersion { get; set; }
	}
}