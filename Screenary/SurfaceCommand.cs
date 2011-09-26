using System;
using System.IO;

namespace Screenary
{
	public class SurfaceCommand
	{
		public FreeRDP freerdp;
		
		const UInt16 CMDTYPE_SET_SURFACE_BITS = 1;
		const UInt16 CMDTYPE_STREAM_SURFACE_BITS = 6;
		const UInt16 CMDTYPE_FRAME_MARKER = 4;
		
		public struct SurfaceBitsCommand
		{
			public UInt16 destLeft;
			public UInt16 destTop;
			public UInt16 destRight;
			public UInt16 destBottom;
			public Byte bpp;
			public Byte codecID;
			public UInt16 width;
			public UInt16 height;
			public UInt32 bitmapDataLength;
			public byte[] bitmapData;
		}
		
		public struct FrameMarkerCommand
		{
			public UInt16 frameAction;
			public UInt32 frameId;
		}
		
		public SurfaceCommand ()
		{
		}
		
		public void readSurfaceBitsCommand(BinaryReader fp)
		{
			SurfaceBitsCommand cmd = new SurfaceBitsCommand();
	
			cmd.destLeft = fp.ReadUInt16(); /* destLeft */
			cmd.destTop = fp.ReadUInt16(); /* destTop */
			cmd.destRight = fp.ReadUInt16(); /* destRight */
			cmd.destBottom = fp.ReadUInt16(); /* destBottom */
			cmd.bpp = fp.ReadByte(); /* bpp */
			fp.ReadByte(); /* Reserved1 */
			fp.ReadByte(); /* Reserved2 */
			cmd.codecID = fp.ReadByte(); /* codecID */
			cmd.width = fp.ReadUInt16(); /* width */
			cmd.height = fp.ReadUInt16(); /* height */
			cmd.bitmapDataLength = fp.ReadUInt32(); /* bitmapDataLength */
			cmd.bitmapData = fp.ReadBytes((int) cmd.bitmapDataLength); /* bitmapData */
			
			freerdp = new FreeRDP();
			freerdp.RfxProcessMessage(cmd.bitmapData, (int) cmd.bitmapDataLength);
		}
		
		public void readFrameMarkerCommand(BinaryReader fp)
		{
			FrameMarkerCommand cmd = new FrameMarkerCommand();
			
			cmd.frameAction = fp.ReadUInt16(); /* frameAction */
			cmd.frameId = fp.ReadUInt32(); /* frameId */
		}
		
		public void test()
		{
			UInt16 cmdType;
			
			BinaryReader fp;
			string filename = "data/rfx/rfx.bin";
			
			fp = new BinaryReader(File.Open(filename, FileMode.Open));
			
			cmdType = fp.ReadUInt16();
			
			Console.WriteLine("cmdType:" + cmdType);
			
			switch (cmdType)
			{
				case CMDTYPE_SET_SURFACE_BITS:
				case CMDTYPE_STREAM_SURFACE_BITS:
					readSurfaceBitsCommand(fp);
					break;
				
				case CMDTYPE_FRAME_MARKER:
					readFrameMarkerCommand(fp);
					break;
			}
			
			fp.Close();
		}
	}
}

