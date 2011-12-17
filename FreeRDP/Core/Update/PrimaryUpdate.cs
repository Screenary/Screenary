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
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultiDstBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public UInt32 numRectangles;
		public UInt32 cbData;
		//public fixed DeltaRect rectangles[45];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultiPatBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public UInt32 backColor;
		public UInt32 foreColor;
		public rdpBrush brush;
		public UInt32 numRectangles;
		public UInt32 cbData;
		//public fixed DeltaRect rectangles[45];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultiScrBltOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public Int32 nXSrc;
		public Int32 nYSrc;
		public UInt32 numRectangles;
		public UInt32 cbData;
		//public fixed DeltaRect rectangles[45];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultiOpaqueRectOrder
	{
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 color;
		public UInt32 numRectangles;
		public UInt32 cbData;
		//public fixed DeltaRect rectangles[45];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MultiDrawNineGridOrder
	{
		public Int32 srcLeft;
		public Int32 srcTop;
		public Int32 srcRight;
		public Int32 srcBottom;
		public UInt32 bitmapId;
		public UInt32 nDeltaEntries;
		public UInt32 cbData;
		public byte* codeDeltaList;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct LineToOrder
	{
		public UInt32 backMode;
		public Int32 nXStart;
		public Int32 nYStart;
		public Int32 nXEnd;
		public Int32 nYEnd;
		public UInt32 backColor;
		public UInt32 bRop2;
		public UInt32 penStyle;
		public UInt32 penWidth;
		public UInt32 penColor;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DeltaPoint
	{
		public Int32 x;
		public Int32 y;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PolylineOrder
	{
		public Int32 xStart;
		public Int32 yStart;
		public UInt32 bRop2;
		public UInt32 penColor;
		public UInt32 numPoints;
		public UInt32 cbData;
		public DeltaPoint* points;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MemBltOrder
	{
		public UInt32 cacheId;
		public UInt32 colorIndex;
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public Int32 nXSrc;
		public Int32 nYSrc;
		public UInt32 cacheIndex;
		public rdpBitmap* bitmap;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Mem3BltOrder
	{
		public UInt32 cacheId;
		public UInt32 colorIndex;
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nWidth;
		public Int32 nHeight;
		public UInt32 bRop;
		public Int32 nXSrc;
		public Int32 nYSrc;
		public UInt32 backColor;
		public UInt32 foreColor;
		public rdpBrush brush;
		public UInt32 cacheIndex;
		public rdpBitmap* bitmap;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SaveBitmapOrder
	{
		public UInt32 savedBitmapPosition;
		public Int32 nLeftRect;
		public Int32 nTopRect;
		public Int32 nRightRect;
		public Int32 nBottomRect;
		public UInt32 operation;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GlyphFragmentIndex
	{
		public UInt32 index;
		public UInt32 delta;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GlyphFragment
	{
		public UInt32 operation;
		public UInt32 index;
		public UInt32 size;
		public UInt32 nindices;
		public GlyphFragmentIndex* indices;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GlyphIndexOrder
	{
		public UInt32 cacheId;
		public UInt32 flAccel;
		public UInt32 ulCharInc;
		public UInt32 fOpRedundant;
		public UInt32 backColor;
		public UInt32 foreColor;
		public Int32 bkLeft;
		public Int32 bkTop;
		public Int32 bkRight;
		public Int32 bkBottom;
		public Int32 opLeft;
		public Int32 opTop;
		public Int32 opRight;
		public Int32 opBottom;
		public rdpBrush brush;
		public Int32 x;
		public Int32 y;
		public UInt32 cbData;
		public fixed byte data[256];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FastIndexOrder
	{
		public UInt32 cacheId;
		public UInt32 flAccel;
		public UInt32 ulCharInc;
		public UInt32 backColor;
		public UInt32 foreColor;
		public Int32 bkLeft;
		public Int32 bkTop;
		public Int32 bkRight;
		public Int32 bkBottom;
		public Int32 opLeft;
		public Int32 opTop;
		public Int32 opRight;
		public Int32 opBottom;
		public int opaqueRect;
		public Int32 x;
		public Int32 y;
		public UInt32 cbData;
		public fixed byte data[256];
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FastGlyphOrder
	{
		public UInt32 cacheId;
		public UInt32 flAccel;
		public UInt32 ulCharInc;
		public UInt32 backColor;
		public UInt32 foreColor;
		public Int32 bkLeft;
		public Int32 bkTop;
		public Int32 bkRight;
		public Int32 bkBottom;
		public Int32 opLeft;
		public Int32 opTop;
		public Int32 opRight;
		public Int32 opBottom;
		public Int32 x;
		public Int32 y;
		public UInt32 cbData;
		public fixed byte data[256];
		public void* glyphData;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PolygonSCOrder
	{
		public Int32 xStart;
		public Int32 yStart;
		public UInt32 bRop2;
		public UInt32 fillMode;
		public UInt32 brushColor;
		public UInt32 nDeltaEntries;
		public UInt32 cbData;
		public byte* codeDeltaList;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct PolygonCBOrder
	{
		public Int32 xStart;
		public Int32 yStart;
		public UInt32 bRop2;
		public UInt32 fillMode;
		public UInt32 backColor;
		public UInt32 foreColor;
		public rdpBrush brush;
		public UInt32 nDeltaEntries;
		public UInt32 cbData;
		public byte* codeDeltaList;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EllipseSCOrder
	{
		public Int32 leftRect;
		public Int32 topRect;
		public Int32 rightRect;
		public Int32 bottomRect;
		public UInt32 bRop2;
		public UInt32 fillMode;
		public UInt32 color;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EllipseCBOrder
	{
		public Int32 leftRect;
		public Int32 topRect;
		public Int32 rightRect;
		public Int32 bottomRect;
		public UInt32 bRop2;
		public UInt32 fillMode;
		public UInt32 backColor;
		public UInt32 foreColor;
		public rdpBrush brush;
	}
	
	public unsafe delegate void pDstBlt(rdpContext* context, DstBltOrder* dstblt);
	public unsafe delegate void pPatBlt(rdpContext* context, PatBltOrder* patblt);
	public unsafe delegate void pScrBlt(rdpContext* context, ScrBltOrder* scrblt);
	public unsafe delegate void pOpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect);
	public unsafe delegate void pDrawNineGrid(rdpContext* context, DrawNineGridOrder* drawNineGrid);
	public unsafe delegate void pMultiDstBlt(rdpContext* context, MultiDstBltOrder* multi_dstblt);
	public unsafe delegate void pMultiPatBlt(rdpContext* context, MultiPatBltOrder* multi_patblt);
	public unsafe delegate void pMultiScrBlt(rdpContext* context, MultiScrBltOrder* multi_scrblt);
	public unsafe delegate void pMultiOpaqueRect(rdpContext* context, MultiOpaqueRectOrder* multi_opaque_rect);
	public unsafe delegate void pMultiDrawNineGrid(rdpContext* context, MultiDrawNineGridOrder* multi_draw_nine_grid);
	public unsafe delegate void pLineTo(rdpContext* context, LineToOrder* line_to);
	public unsafe delegate void pPolyline(rdpContext* context, PolylineOrder* polyline);
	public unsafe delegate void pMemBlt(rdpContext* context, MemBltOrder* memblt);
	public unsafe delegate void pMem3Blt(rdpContext* context, Mem3BltOrder* mem3blt);
	public unsafe delegate void pSaveBitmap(rdpContext* context, SaveBitmapOrder* save_bitmap);
	public unsafe delegate void pGlyphIndex(rdpContext* context, GlyphIndexOrder* glyph_index);
	public unsafe delegate void pFastIndex(rdpContext* context, FastIndexOrder* fast_index);
	public unsafe delegate void pFastGlyph(rdpContext* context, FastGlyphOrder* fast_glyph);
	public unsafe delegate void pPolygonSC(rdpContext* context, PolygonSCOrder* polygon_sc);
	public unsafe delegate void pPolygonCB(rdpContext* context, PolygonCBOrder* polygon_cb);
	public unsafe delegate void pEllipseSC(rdpContext* context, EllipseSCOrder* ellipse_sc);
	public unsafe delegate void pEllipseCB(rdpContext* context, EllipseCBOrder* ellipse_cb);
	
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
	
	public unsafe interface IPrimaryUpdate
	{		
		void DstBlt(rdpContext* context, DstBltOrder* dstblt);
		void PatBlt(rdpContext* context, PatBltOrder* patblt);
		void ScrBlt(rdpContext* context, ScrBltOrder* scrblt);
		void OpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect);
		void DrawNineGrid(rdpContext* context, DrawNineGridOrder* drawNineGrid);
		void MultiDstBlt(rdpContext* context, MultiDstBltOrder* multi_dstblt);
		void MultiPatBlt(rdpContext* context, MultiPatBltOrder* multi_patblt);
		void MultiScrBlt(rdpContext* context, MultiScrBltOrder* multi_scrblt);
		void MultiOpaqueRect(rdpContext* context, MultiOpaqueRectOrder* multi_opaque_rect);
		void MultiDrawNineGrid(rdpContext* context, MultiDrawNineGridOrder* multi_draw_nine_grid);
		void LineTo(rdpContext* context, LineToOrder* line_to);
		void Polyline(rdpContext* context, PolylineOrder* polyline);
		void MemBlt(rdpContext* context, MemBltOrder* memblt);
		void Mem3Blt(rdpContext* context, Mem3BltOrder* mem3blt);
		void SaveBitmap(rdpContext* context, SaveBitmapOrder* save_bitmap);
		void GlyphIndex(rdpContext* context, GlyphIndexOrder* glyph_index);
		void FastIndex(rdpContext* context, FastIndexOrder* fast_index);
		void FastGlyph(rdpContext* context, FastGlyphOrder* fast_glyph);
		void PolygonSC(rdpContext* context, PolygonSCOrder* polygon_sc);
		void PolygonCB(rdpContext* context, PolygonCBOrder* polygon_cb);
		void EllipseSC(rdpContext* context, EllipseSCOrder* ellipse_sc);
		void EllipseCB(rdpContext* context, EllipseCBOrder* ellipse_cb);
	}
	
	public unsafe class PrimaryUpdate
	{
		private freerdp* instance;
		private rdpContext* context;
		private rdpUpdate* update;
		private rdpPrimaryUpdate* primary;
		
		private pDstBlt DstBlt;
		private pPatBlt PatBlt;
		private pScrBlt ScrBlt;
		private pOpaqueRect OpaqueRect;
		private pDrawNineGrid DrawNineGrid;
		private pMultiDstBlt MultiDstBlt;
		private pMultiPatBlt MultiPatBlt;
		private pMultiScrBlt MultiScrBlt;
		private pMultiOpaqueRect MultiOpaqueRect;
		private pMultiDrawNineGrid MultiDrawNineGrid;
		private pLineTo LineTo;
		private pPolyline Polyline;
		private pMemBlt MemBlt;
		private pMem3Blt Mem3Blt;
		private pSaveBitmap SaveBitmap;
		private pGlyphIndex GlyphIndex;
		private pFastIndex FastIndex;
		private pFastGlyph FastGlyph;
		private pPolygonSC PolygonSC;
		private pPolygonCB PolygonCB;
		private pEllipseSC EllipseSC;
		private pEllipseCB EllipseCB;
		
		public PrimaryUpdate(rdpContext* context)
		{
			this.context = context;
			this.instance = context->instance;
			this.update = instance->update;
			this.primary = update->primary;
		}
		
		public void RegisterInterface(IPrimaryUpdate iPrimary)
		{
			DstBlt = new pDstBlt(iPrimary.DstBlt);
			PatBlt = new pPatBlt(iPrimary.PatBlt);
			ScrBlt = new pScrBlt(iPrimary.ScrBlt);
			OpaqueRect = new pOpaqueRect(iPrimary.OpaqueRect);
			DrawNineGrid = new pDrawNineGrid(iPrimary.DrawNineGrid);
			MultiDstBlt = new pMultiDstBlt(iPrimary.MultiDstBlt);
			MultiPatBlt = new pMultiPatBlt(iPrimary.MultiPatBlt);
			MultiScrBlt = new pMultiScrBlt(iPrimary.MultiScrBlt);
			MultiOpaqueRect = new pMultiOpaqueRect(iPrimary.MultiOpaqueRect);
			MultiDrawNineGrid = new pMultiDrawNineGrid(iPrimary.MultiDrawNineGrid);
			LineTo = new pLineTo(iPrimary.LineTo);
			Polyline = new pPolyline(iPrimary.Polyline);
			MemBlt = new pMemBlt(iPrimary.MemBlt);
			Mem3Blt = new pMem3Blt(iPrimary.Mem3Blt);
			SaveBitmap = new pSaveBitmap(iPrimary.SaveBitmap);
			GlyphIndex = new pGlyphIndex(iPrimary.GlyphIndex);
			FastIndex = new pFastIndex(iPrimary.FastIndex);
			FastGlyph = new pFastGlyph(iPrimary.FastGlyph);
			PolygonSC = new pPolygonSC(iPrimary.PolygonSC);
			PolygonCB = new pPolygonCB(iPrimary.PolygonCB);
			EllipseSC = new pEllipseSC(iPrimary.EllipseSC);
			EllipseCB = new pEllipseCB(iPrimary.EllipseCB);
			
			primary->DstBlt = Marshal.GetFunctionPointerForDelegate(DstBlt);
			primary->PatBlt = Marshal.GetFunctionPointerForDelegate(PatBlt);
			primary->ScrBlt = Marshal.GetFunctionPointerForDelegate(ScrBlt);
			primary->OpaqueRect = Marshal.GetFunctionPointerForDelegate(OpaqueRect);
			primary->DrawNineGrid = Marshal.GetFunctionPointerForDelegate(DrawNineGrid);
			primary->MultiDstBlt = Marshal.GetFunctionPointerForDelegate(MultiDstBlt);
			primary->MultiPatBlt = Marshal.GetFunctionPointerForDelegate(MultiPatBlt);
			primary->MultiScrBlt = Marshal.GetFunctionPointerForDelegate(MultiScrBlt);
			primary->MultiOpaqueRect = Marshal.GetFunctionPointerForDelegate(MultiOpaqueRect);
			primary->MultiDrawNineGrid = Marshal.GetFunctionPointerForDelegate(MultiDrawNineGrid);
			primary->LineTo = Marshal.GetFunctionPointerForDelegate(LineTo);
			primary->Polyline = Marshal.GetFunctionPointerForDelegate(Polyline);
			primary->MemBlt = Marshal.GetFunctionPointerForDelegate(MemBlt);
			primary->Mem3Blt = Marshal.GetFunctionPointerForDelegate(Mem3Blt);
			primary->SaveBitmap = Marshal.GetFunctionPointerForDelegate(SaveBitmap);
			primary->GlyphIndex = Marshal.GetFunctionPointerForDelegate(GlyphIndex);
			primary->FastIndex = Marshal.GetFunctionPointerForDelegate(FastIndex);
			primary->FastGlyph = Marshal.GetFunctionPointerForDelegate(FastGlyph);
			primary->PolygonSC = Marshal.GetFunctionPointerForDelegate(PolygonSC);
			primary->PolygonCB = Marshal.GetFunctionPointerForDelegate(PolygonCB);
			primary->EllipseSC = Marshal.GetFunctionPointerForDelegate(EllipseSC);
			primary->EllipseCB = Marshal.GetFunctionPointerForDelegate(EllipseCB);
		}
	}
}

