using System;
using System.IO;
using System.Collections;

namespace Screenary
{
	public class PcapRecord
	{
		public byte[] Buffer { get; set; }
		
		public int Length
		{
			get { return Buffer.Length; }
		}
		
		public TimeSpan Time { get; set; }
		
		public PcapRecord(byte[] buffer, TimeSpan time)
		{
			this.Buffer = buffer;
			this.Time = time;
		}
	}
}

