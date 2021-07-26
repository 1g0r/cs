using Mindscan.Media.Messages.Collector;

namespace Mindscan.Media.Messages.Smi
{
	public sealed class CleanMaterialMessage : MessageBase
	{
		public CleanMaterialMessage()
		{
			
		}
		public CleanMaterialMessage(MessageBase message)
			:base(message)
		{
			
		}
		public MaterialData Material { get; set; }
	}
}