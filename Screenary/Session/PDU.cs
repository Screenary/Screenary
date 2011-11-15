using System;

namespace Screenary
{
	public class PDU
	{
		public byte[] Buffer { get; set; }
		
		public int Length
		{
			get { return Buffer.Length; }
		}
		
		public byte Type { get; set; }
		
		public UInt16 ChannelId { get; set; }
		
		public PDU(byte[] buffer, UInt16 channelId, byte type)
		{
			this.Buffer = buffer;
			this.ChannelId = channelId;
			this.Type = type;
		}
	}
}
