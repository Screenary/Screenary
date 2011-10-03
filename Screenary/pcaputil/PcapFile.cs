using System;
namespace Screenary
{
	public class PcapFile
	{
		public char name { get; set; }
		public uint fileSize { get; set; }
		public uint recordCount { get; set; }
		public PcapHeader pcapHeader { get; set; }
	}
}

