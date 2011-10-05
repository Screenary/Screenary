using System;
using System.IO;

namespace FreeRDP
{
	public class StreamSurfaceBitsCommand : SurfaceCommand
	{
		private byte[] buffer;
		
		private UInt16 destLeft;
		private UInt16 destTop;
		private UInt16 destRight;
		private UInt16 destBottom;
		private Byte bpp;
		private Byte codecID;
		private UInt16 width;
		private UInt16 height;
		private UInt32 bitmapDataLength;
		private byte[] bitmapData;
		
		public StreamSurfaceBitsCommand()
		{
			buffer = new byte[4096 * 4];
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
		
		public override void Execute(SurfaceReceiver receiver)
		{
			if (codecID != CODEC_ID_REMOTEFX)
				return;
			
			int x, y;
			int tx, ty;
			int width, height;
			Gdk.Pixbuf pixbuf;
			RfxMessage rfxMsg = receiver.rfx.ParseMessage(bitmapData, bitmapDataLength);
			
			x = y = 0;
			width = height = 64;
			
			while (rfxMsg.HasNextTile())
			{
				rfxMsg.GetNextTile(buffer, ref x, ref y);
				
				tx = x + destLeft;
				ty = y + destTop;
				
				pixbuf = new Gdk.Pixbuf(buffer, Gdk.Colorspace.Rgb, true, 8, 64, 64, 64 * 4);
				pixbuf.CopyArea(0, 0, 64, 64, receiver.surface, tx, ty);
				receiver.InvalidateRect(tx, ty, width, height);
			}
			
			/*
			while (rfxMsg.HasNextRect())
			{
				rfxMsg.GetNextRect(ref x, ref y, ref width, ref height);
				receiver.InvalidateRect(x, y, width, height);
			}
			*/
		}
	}
}

