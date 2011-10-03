using System;
using System.IO;
using System.Collections;

namespace Screenary
{

	public class PCapUtil
	{
		public const uint MAGIC_NUMBER = 0xa1b2c3d4;
		public string filepath { get; set; }
		public BinaryReader binaryReader { get; set; }

		public PCapUtil (string filepath)
		{
			this.filepath = filepath;
		}
		
		static void Main (string[] args)
		{
			string filepath = "test.pcap";
			System.Console.WriteLine ("PCAP Util reading: " + filepath + "\n");
			
			PCapUtil util = new PCapUtil (filepath);
			
			try {
				util.open();
				util.readHeader();
				
				while (util.hasNextRecord()) {
					PcapRecord pcapRecord = util.readNextRecord();
					Console.WriteLine ("Packet is {0} bytes", pcapRecord.packet.length);
				}
				
				util.close();
			} catch (System.IO.FileNotFoundException e) {
				Console.WriteLine (e.Message);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				util.close();
			}
		}

		public PcapHeader readHeader()
		{
			if (hasNextRecord()) {
				PcapHeader pcapHeader = new PcapHeader();
				pcapHeader.magicNumber = binaryReader.ReadUInt32();
				
				if (pcapHeader.magicNumber != MAGIC_NUMBER) {
					throw new System.Exception ("\nERROR: pcapHeader.magicnumber != MAGIC_NUMBER\n");
				}
				
				pcapHeader.versionMajor = binaryReader.ReadUInt16();
				pcapHeader.versionMinor = binaryReader.ReadUInt16();
				pcapHeader.thiszone = binaryReader.ReadInt32();
				pcapHeader.sigfigs = binaryReader.ReadUInt32();
				pcapHeader.snaplen = binaryReader.ReadUInt32();
				pcapHeader.network = binaryReader.ReadUInt32();
				
				return pcapHeader;
			}
			
			return null;
			
		}

		private PcapRecordHeader readNextRecordHeader()
		{
			if (hasNextRecord()) {
				PcapRecordHeader pcapRecordHeader = new PcapRecordHeader();
				pcapRecordHeader.tsSec = binaryReader.ReadUInt32();
				pcapRecordHeader.tsUsec = binaryReader.ReadUInt32();
				pcapRecordHeader.inclLen = binaryReader.ReadUInt32();
				pcapRecordHeader.origLen = binaryReader.ReadUInt32();
				
				return pcapRecordHeader;
			}
			
			return null;
		}

		public PcapRecord readNextRecord()
		{
			if (hasNextRecord()) {
				PcapRecordHeader pcapRecordHeader = readNextRecordHeader();
				PcapRecord pcapRecord = new PcapRecord();
				pcapRecord.pcapRecordHeader = pcapRecordHeader;
				pcapRecord.packet = new Packet();
				pcapRecord.packet.length = pcapRecordHeader.inclLen;
				pcapRecord.packet.data = binaryReader.ReadBytes ((int)pcapRecord.packet.length);
				
				return pcapRecord;
			}
			
			return null;
		}

		public Boolean hasNextRecord()
		{
			return (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length);
		}

		public void open()
		{
			FileStream inStream = File.OpenRead (filepath);
			binaryReader = new BinaryReader (inStream);
		}

		public void close()
		{
			binaryReader.Close();
		}
	}
}

