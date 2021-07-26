namespace Mindscan.Media.Messages.Collector
{
	public sealed class CollectMaterialMessage : MessageBase
	{
		public CollectMaterialMessage()
		{
			
		}

		public CollectMaterialMessage(MessageBase message)
			:base(message)
		{
			
		}
		public MaterialData Material { get; set; }
		public string OriginalJson { get; set; }
	}
}