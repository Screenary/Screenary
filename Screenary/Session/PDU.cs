using System;

namespace Screenary
{
	public class PDU
	{
		public const int PDU_HEADER_LENGTH = 6;
		public const int PDU_MAX_FRAG_SIZE = 0x3FFF;
		public const int PDU_MAX_PAYLOAD_SIZE = (PDU_MAX_FRAG_SIZE - PDU_HEADER_LENGTH);
		
		public const UInt16 PDU_CHANNEL_SESSION = 0x00;
		public const UInt16 PDU_CHANNEL_UPDATE = 0x01;
		public const UInt16 PDU_CHANNEL_INPUT = 0x02;
		
		public const byte PDU_UPDATE_SURFACE = 0x01;
		
		public const byte PDU_FRAGMENT_SINGLE = 0x00;
		public const byte PDU_FRAGMENT_FIRST = 0x01;
		public const byte PDU_FRAGMENT_NEXT = 0x02;
		public const byte PDU_FRAGMENT_LAST = 0x03;
		
		private PcapRecord record;
		
		public PDU(PcapRecord record)
		{
			this.record = record;
		}
		
		public PcapRecord getRecord()
		{
			return record;	
		}
	}
}
