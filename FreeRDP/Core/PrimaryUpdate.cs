using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pDstBlt(rdpContext* context, IntPtr dstblt);
	public unsafe delegate void pPatBlt(rdpContext* context, IntPtr patblt);
	public unsafe delegate void pScrBlt(rdpContext* context, IntPtr scrblt);
	public unsafe delegate void pOpaqueRect(rdpContext* context, IntPtr opaque_rect);
	public unsafe delegate void pDrawNineGrid(rdpContext* context, IntPtr draw_nine_grid);
	public unsafe delegate void pMultiDstBlt(rdpContext* context, IntPtr multi_dstblt);
	public unsafe delegate void pMultiPatBlt(rdpContext* context, IntPtr multi_patblt);
	public unsafe delegate void pMultiScrBlt(rdpContext* context, IntPtr multi_scrblt);
	public unsafe delegate void pMultiOpaqueRect(rdpContext* context, IntPtr multi_opaque_rect);
	public unsafe delegate void pMultiDrawNineGrid(rdpContext* context, IntPtr multi_draw_nine_grid);
	public unsafe delegate void pLineTo(rdpContext* context, IntPtr line_to);
	public unsafe delegate void pPolyline(rdpContext* context, IntPtr polyline);
	public unsafe delegate void pMemBlt(rdpContext* context, IntPtr memblt);
	public unsafe delegate void pMem3Blt(rdpContext* context, IntPtr memblt);
	public unsafe delegate void pSaveBitmap(rdpContext* context, IntPtr save_bitmap);
	public unsafe delegate void pGlyphIndex(rdpContext* context, IntPtr glyph_index);
	public unsafe delegate void pFastIndex(rdpContext* context, IntPtr fast_index);
	public unsafe delegate void pFastGlyph(rdpContext* context, IntPtr fast_glyph);
	public unsafe delegate void pPolygonSC(rdpContext* context, IntPtr polygon_sc);
	public unsafe delegate void pPolygonCB(rdpContext* context, IntPtr polygon_cb);
	public unsafe delegate void pEllipseSC(rdpContext* context, IntPtr ellipse_sc);
	public unsafe delegate void pEllipseCB(rdpContext* context, IntPtr ellipse_cb);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpPrimaryUpdate
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public IntPtr DstBlt;
		public IntPtr PatBlt;
		public IntPtr ScrBlt;
		public IntPtr OpaqueRect;
		public IntPtr DrawNineGrid;
		public IntPtr MultiDstBlt;
		public IntPtr MultiPatBlt;
		public IntPtr MultiScrBlt;
		public IntPtr MultiOpaqueRect;
		public IntPtr MultiDrawNineGrid;
		public IntPtr LineTo;
		public IntPtr Polyline;
		public IntPtr MemBlt;
		public IntPtr Mem3Blt;
		public IntPtr SaveBitmap;
		public IntPtr GlyphIndex;
		public IntPtr FastIndex;
		public IntPtr FastGlyph;
		public IntPtr PolygonSC;
		public IntPtr PolygonCB;
		public IntPtr EllipseSC;
		public IntPtr EllipseCB;
		public fixed UInt32 paddingB[48-38];
	}
}

