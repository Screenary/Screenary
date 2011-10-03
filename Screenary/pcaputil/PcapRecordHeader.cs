using System;
namespace Screenary
{
	public class PcapRecordHeader
	{
		public uint tsSec { get; set; }		/* timestamp seconds */
		public uint tsUsec { get; set; }	/* timestamp microseconds */
		public uint inclLen { get; set; }	/* number of octets of packet saved in file */
		public uint origLen { get; set; }	/* actual length of packet */

		public String toString()
		{
			return String.Format ("\ntsSec = {0}" + "\ntsUsec = {1}" + "\ninclLen = {2}" + "\norigLen = {3}", this.tsSec, this.tsUsec, this.inclLen, this.origLen);
		}
	}
}

