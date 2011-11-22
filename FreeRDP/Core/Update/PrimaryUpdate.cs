using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpBrush
	{
		public UInt32 x;
		public UInt32 y;
		public UInt32 bpp;
		public UInt32 style;
		public UInt32 hatch;
		public UInt32 index;
		public UInt32 data;
		public fixed byte p8x8[8];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct OrderInfo
	{
		public UInt32 orderType;
		public UInt32 fieldFlags;
		public rdpBounds* bounds;
		public Int32 deltaBoundLeft;
		public Int32 deltaBoundTop;
		public Int32 deltaBoundRight;
		public Int32 deltaBoundBottom;
		public int deltaCoordinates;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DstBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PatBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public UInt32 backColor;
		public UInt32 foreColor;
		public rdpBrush* brush;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ScrBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public Int32 nXSrc;
		public Int32 nYSrc;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct OpaqueRectOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 color;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawNineGridOrder
	{
		public Int32 srcLeft;
		public Int32 srcTop;
		public Int32 srcRight;
		public Int32 srcBottom;
		public UInt32 bitmapId;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DeltaRect
	{
		public Int32 left;
		public Int32 top;
		public Int32 width;
		public Int32 height;
	}
	
	public unsafe delegate void pDstBlt(rdpContext* context, DstBltOrder* dstblt);
	public unsafe delegate void pPatBlt(rdpContext* context, PatBltOrder* patblt);
	public unsafe delegate void pScrBlt(rdpContext* context, ScrBltOrder* scrblt);
	public unsafe delegate void pOpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect);
	public unsafe delegate void pDrawNineGrid(rdpContext* context, DrawNineGridOrder* drawNineGrid);
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

