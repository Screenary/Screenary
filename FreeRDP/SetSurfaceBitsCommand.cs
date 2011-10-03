using System;
using System.IO;

namespace FreeRDP
{
	public class SetSurfaceBitsCommand : SurfaceCommand
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
		
		public SetSurfaceBitsCommand()
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
		
		public override void Execute(Gdk.Window window, Gdk.Pixbuf surface)
		{
			int x, y;
			Gdk.Pixbuf pixbuf;
			Rfx rfx = new Rfx();
			RfxMessage rfxMsg = rfx.ParseMessage(bitmapData, bitmapDataLength);
			
			x = y = 0;
			
			while (rfxMsg.HasNextTile())
			{
				rfxMsg.GetNextTile(buffer, ref x, ref y);
				pixbuf = new Gdk.Pixbuf(buffer, Gdk.Colorspace.Rgb, true, 8, 64, 64, 64 * 4);
				pixbuf.CopyArea(0, 0, 64, 64, surface, x, y);
				window.InvalidateRect(new Gdk.Rectangle(x, y, 64, 64), true);
			}
		}
	}
}

