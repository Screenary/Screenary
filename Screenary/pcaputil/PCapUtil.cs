using System;
using System.IO;
using System.Collections;
		
namespace Screenary
{

	public class PCapUtil
	{
		private const uint MAGIC_NUMBER = 0xa1b2c3d4;
        private string _filepath;
		private BinaryReader _binaryReader;
				
		public PCapUtil (string filepath)
		{
			_filepath = filepath;
		}
		
		public string Filepath
		{
			get
			{
				return _filepath;
			}
			
			set
			{
				_filepath = value;
			}
		}
				
		static void Main(string[] args)
        {
            string filepath = "test.pcap";
			System.Console.WriteLine("PCAP Util reading: " + filepath + "\n");

			PCapUtil util = new PCapUtil(filepath);
			
			try 
			{
				util.pcapOpen();
				util.pcapReadHeader();
	
				while (util.pcapHasNextRecord())
				{
					PcapRecord pcapRecord = util.pcapReadNextRecord();
					Console.WriteLine("Packet is {0} bytes", pcapRecord.Packet.Length);
				}
				
				util.pcapClose();
			}
			catch (System.IO.FileNotFoundException e)
			{
				Console.WriteLine(e.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				util.pcapClose();
			}		
        }	
		
		public PcapHeader pcapReadHeader()
		{
			if (pcapHasNextRecord())
			{
				PcapHeader pcapHeader = new PcapHeader();
				pcapHeader.MagicNumber = _binaryReader.ReadUInt32();
				
				if(pcapHeader.MagicNumber != MAGIC_NUMBER)
				{
					throw new System.Exception("\nERROR: pcapHeader.magic_number != MAGIC_NUMBER\n");
				}
				
				pcapHeader.VersionMajor = _binaryReader.ReadUInt16();
				pcapHeader.VersionMinor = _binaryReader.ReadUInt16();
				pcapHeader.Thiszone = _binaryReader.ReadInt32();
				pcapHeader.Sigfigs = _binaryReader.ReadUInt32();
				pcapHeader.Snaplen = _binaryReader.ReadUInt32();
				pcapHeader.Network = _binaryReader.ReadUInt32();
						
				return pcapHeader;
			}
			
			return null;
					
		}
		
		private PcapRecordHeader pcapReadNextRecordHeader()
		{
			if (pcapHasNextRecord())
			{
				PcapRecordHeader pcapRecordHeader = new PcapRecordHeader();
				pcapRecordHeader.TsSec = _binaryReader.ReadUInt32();
				pcapRecordHeader.TsUsec = _binaryReader.ReadUInt32();
				pcapRecordHeader.InclLen = _binaryReader.ReadUInt32();
				pcapRecordHeader.OrigLen = _binaryReader.ReadUInt32();
				
				return pcapRecordHeader;
			}
			
			return null;
		 }
		
		public PcapRecord pcapReadNextRecord()
		{
			if (pcapHasNextRecord())
			{
				PcapRecordHeader pcapRecordHeader = pcapReadNextRecordHeader();
				PcapRecord pcapRecord = new PcapRecord();
				pcapRecord.PcapRecordHeader = pcapRecordHeader;
				pcapRecord.Packet = new Packet();
				pcapRecord.Packet.Length = pcapRecordHeader.InclLen;
				pcapRecord.Packet.Data = _binaryReader.ReadBytes((int)pcapRecord.Packet.Length);
								
				return pcapRecord;
			}
			
			return null;
		}
		
		public Boolean pcapHasNextRecord()
		{
			return (_binaryReader.BaseStream.Position < _binaryReader.BaseStream.Length);	
		}
		
		public void pcapOpen()
		{
			FileStream inStream = File.OpenRead(_filepath);
		    _binaryReader = new BinaryReader(inStream);
		}
		
		public void pcapClose()
		{
			_binaryReader.Close();
		}
	}
}

