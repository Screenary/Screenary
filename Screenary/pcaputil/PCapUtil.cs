using System;
using System.IO;
using System.Collections;

namespace Screenary
{
	public class PCapUtil
	{
		public const uint MAGIC_NUMBER = 0xA1B2C3D4;
		public string filepath { get; set; }
		public BinaryReader binaryReader { get; set; }

		public PCapUtil(string filepath)
		{
			this.filepath = filepath;
		}

		public PcapHeader readHeader()
		{
			if (hasNextRecord())
			{
				PcapHeader pcapHeader = new PcapHeader();
				pcapHeader.magicNumber = binaryReader.ReadUInt32();
				
				if (pcapHeader.magicNumber != MAGIC_NUMBER)
					throw new System.Exception ("\nERROR: pcapHeader.magicnumber != MAGIC_NUMBER\n");
				
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
			if (hasNextRecord())
			{
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
			if (hasNextRecord())
			{
				PcapRecordHeader pcapRecordHeader = readNextRecordHeader();
				PcapRecord pcapRecord = new PcapRecord();
				pcapRecord.pcapRecordHeader = pcapRecordHeader;
				pcapRecord.packet = new Packet();
				pcapRecord.packet.length = pcapRecordHeader.inclLen;
				pcapRecord.packet.data = binaryReader.ReadBytes((int) pcapRecord.packet.length);
				
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
			FileStream inStream = File.OpenRead(filepath);
			binaryReader = new BinaryReader(inStream);
		}

		public void close()
		{
			binaryReader.Close();
		}
	}
}
