using System;
using System.Collections;

namespace Screenary
{
	public class PcapRecord
	{
		private PcapRecordHeader _pcapRecordHeader;
		private Packet _packet;
		
		public PcapRecord ()
		{
		}
		
		public PcapRecordHeader PcapRecordHeader
		{
			get
			{
				return _pcapRecordHeader;
			}
			
			set
			{
				_pcapRecordHeader = value;
			}
		}
		
		public Packet Packet
		{
			get
			{
				return _packet;
			}
			
			set
			{
				_packet = value;
			}
		}
		
	}
	
}

