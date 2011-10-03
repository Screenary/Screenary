using System;
using System.Collections;

namespace Screenary
{
	public class PcapRecord
	{
		public PcapRecordHeader pcapRecordHeader { get; set; }
		public Packet packet { get; set; }

		public PcapRecord()
		{
		}
		
	}
	
}

