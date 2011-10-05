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
		
		public PcapRecord(byte[] buffer)
		{
			this.Buffer = buffer;
		}
	}
}

