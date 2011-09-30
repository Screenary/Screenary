using System;
namespace Screenary
{
	public class PcapRecordHeader
	{
		
		private uint _tsSec;         /* timestamp seconds */
		private uint _tsUsec;        /* timestamp microseconds */
		private uint _inclLen;       /* number of octets of packet saved in file */
		private uint _origLen;       /* actual length of packet */
		
		public PcapRecordHeader ()
		{
		}
		
		public uint TsSec
		{
			get
			{
				return _tsSec;
			}
			
			set
			{
				_tsSec = value;
			}
		}
		
		public uint TsUsec
		{
			get
			{
				return _tsUsec;
			}
			
			set
			{
				_tsUsec = value;
			}
		}
		
		public uint InclLen
		{
			get
			{
				return _inclLen;
			}
			
			set
			{
				_inclLen = value;
			}
		}
		
		public uint OrigLen
		{
			get
			{
				return _origLen;
			}
			
			set
			{
				_origLen = value;
			}
		}

		public String toString()
		{
			return String.Format("\ntsSec = {0}" + "\ntsUsec = {1}" + "\ninclLen = {2}" + "\norigLen = {3}", 
			                     this.TsSec, this.TsUsec, this.InclLen, this.OrigLen);
		}
	}
}

