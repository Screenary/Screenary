using System;
namespace Screenary
{
	public class PcapFile
	{

		private char _name;
		private uint _fileSize;
		private uint _recordCount;
		private PcapHeader _pcapHeader;
	
		public PcapFile ()
		{
		}
		
		public char Name
		{
			get
			{
				return _name;
			}
			
			set
			{
				_name = value;
			}
		}
		
		public uint FileSize
		{
			get
			{
				return _fileSize;
			}
			
			set
			{
				_fileSize = value;
			}
		}
		
		public uint RecordCount
		{
			get
			{
				return _recordCount;
			}
			
			set
			{
				_recordCount = value;
			}
		}
		
		public PcapHeader PcapHeader
		{
			get
			{
				return _pcapHeader;
			}
			
			set
			{
				_pcapHeader = value;
			}
		}
		
	}
}

