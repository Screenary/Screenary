using System;
namespace Screenary
{
	public class PcapFile
	{

		public char _name;
		public uint _fileSize;
		public uint _recordCount;
		public PcapHeader _pcapHeader;
	
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

