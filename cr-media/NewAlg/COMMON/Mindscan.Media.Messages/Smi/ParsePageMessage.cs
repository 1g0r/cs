using System;

namespace Mindscan.Media.Messages.Smi
{
	public class ParsePageMessage : MessageBase
	{
		public Uri PageUrl { get; set; }
	}
}