using System;
using System.Collections;

namespace Screenary
{
	public class PcapRecord
	{
		public PcapRecordHeader _pcapRecordHeader;
		public Packet _packet;
		
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

