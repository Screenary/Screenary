using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpGraphics
	{
		public rdpContext* context;
		public IntPtr Bitmap_Prototype;
		public IntPtr Pointer_Prototype;
		public IntPtr Glyph_Prototype;
		public fixed UInt32 paddingA[16-4];
	}
	
	public unsafe delegate void pGraphicsRegisterBitmap(rdpGraphics* graphics, rdpBitmap* bitmap);
	public unsafe delegate void pGraphicsRegisterPointer(rdpGraphics* graphics, rdpPointer* pointer);
	public unsafe delegate void pGraphicsRegisterGlyph(rdpGraphics* graphics, rdpGlyph* glyph);
}
