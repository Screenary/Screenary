using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pGlyph_New(rdpContext* context, rdpGlyph* glyph);
	public unsafe delegate void pGlyph_Free(rdpContext* context, rdpGlyph* glyph);
	public unsafe delegate void pGlyph_Draw(rdpContext* context, rdpGlyph* glyph, int x, int y);
	
	public unsafe delegate void pGlyph_BeginDraw(rdpContext* context, rdpGlyph* glyph,
		int x, int y, UInt32 bgcolor, UInt32 fgcolor);
	
	public unsafe delegate void pGlyph_EndDraw(rdpContext* context, rdpGlyph* glyph,
		int x, int y, UInt32 bgcolor, UInt32 fgcolor);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpGlyph
	{
		public IntPtr size;
		public IntPtr New;
		public IntPtr Free;
		public IntPtr Draw;
		public IntPtr BeginDraw;
		public IntPtr EndDraw;
		public fixed UInt32 paddingA[16-6];
		
		public UInt32 x;
		public UInt32 y;
		public UInt32 cx;
		public UInt32 cy;
		public UInt32 cb;
		public IntPtr aj;
		public fixed UInt32 paddingB[32-22];
	}
}
