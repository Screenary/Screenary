using System;
using System.IO;

namespace Screenary
{
	public class SurfaceBitsCommand : SurfaceCommand
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
		
		public SurfaceBitsCommand(UInt16 cmdType)
		{
			this.cmdType = cmdType;
		}
		
		public override void Read(BinaryReader fp)
		{
			destLeft = fp.ReadUInt16(); /* destLeft */
			destTop = fp.ReadUInt16(); /* destTop */
			destRight = fp.ReadUInt16(); /* destRight */
			destBottom = fp.ReadUInt16(); /* destBottom */
			bpp = fp.ReadByte(); /* bpp */
			fp.ReadByte(); /* Reserved1 */
			fp.ReadByte(); /* Reserved2 */
			codecID = fp.ReadByte(); /* codecID */
			width = fp.ReadUInt16(); /* width */
			height = fp.ReadUInt16(); /* height */
			bitmapDataLength = fp.ReadUInt32(); /* bitmapDataLength */
			bitmapData = fp.ReadBytes((int) bitmapDataLength); /* bitmapData */
		}

		public override void Process()
		{
			remotefx.Decode(bitmapData, (int) bitmapDataLength);
		}
	}
}

