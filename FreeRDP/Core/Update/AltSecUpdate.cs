using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct OffscreenDeleteList
	{
		public UInt32 cIndices;
		public UInt16* indices;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CreateOffscreenBitmapOrder
	{
		public UInt32 id;
		public UInt32 cx;
		public UInt32 cy;
		public OffscreenDeleteList deleteList;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SwitchSurfaceOrder
	{
		public UInt32 bitmapId;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct NineGridBitmapInfo
	{
		public UInt32 flFlags;
		public UInt32 ulLeftWidth;
		public UInt32 ulRightWidth;
		public UInt32 ulTopHeight;
		public UInt32 ulBottomHeight;
		public UInt32 crTransparent;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CreateNineGridBitmapOrder
	{
		public UInt32 bitmapBpp;
		public UInt32 bitmapId;
		public UInt32 cx;
		public UInt32 cy;
		public NineGridBitmapInfo* nineGridInfo;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FrameMarkerOrder
	{
		public UInt32 action;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct StreamBitmapFirstOrder
	{
		public UInt32 bitmapFlags;
		public UInt32 bitmapBpp;
		public UInt32 bitmapType;
		public UInt32 bitmapWidth;
		public UInt32 bitmapHeight;
		public UInt32 bitmapSize;
		public UInt32 bitmapBlockSize;
		public byte* bitmapBlock;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct StreamBitmapNextOrder
	{
		public UInt32 bitmapFlags;
		public UInt32 bitmapType;
		public UInt32 bitmapBlockSize;
		public byte* bitmapBlock;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusFirstOrder
	{
		public UInt32 cbSize;
		public UInt32 cbTotalSize;
		public UInt32 cbTotalEmfSize;
		public byte* emfRecords;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusNextOrder
	{
		public UInt32 cbSize;
		public byte* emfRecords;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusEndOrder
	{
		public UInt32 cbSize;
		public UInt32 cbTotalSize;
		public UInt32 cbTotalEmfSize;
		public byte* emfRecords;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusCacheFirstOrder
	{
		public UInt32 flags;
		public UInt32 cacheType;
		public UInt32 cacheIndex;
		public UInt32 cbSize;
		public UInt32 cbTotalSize;
		public byte* emfRecords;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusCacheNextOrder
	{
		public UInt32 flags;
		public UInt32 cacheType;
		public UInt32 cacheIndex;
		public UInt32 cbSize;
		public byte* emfRecords;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct DrawGdiPlusCacheEndOrder
	{
		public UInt32 flags;
		public UInt32 cacheType;
		public UInt32 cacheIndex;
		public UInt32 cbSize;
		public UInt32 cbTotalSize;
		public byte* emfRecords;
	}
	
	public unsafe delegate void pCreateOffscreenBitmap(rdpContext* context, CreateOffscreenBitmapOrder* create_offscreen_bitmap);
	public unsafe delegate void pSwitchSurface(rdpContext* context, SwitchSurfaceOrder* switch_surface);
	public unsafe delegate void pCreateNineGridBitmap(rdpContext* context, CreateNineGridBitmapOrder* create_nine_grid_bitmap);
	public unsafe delegate void pFrameMarker(rdpContext* context, FrameMarkerOrder* frame_marker);
	public unsafe delegate void pStreamBitmapFirst(rdpContext* context, StreamBitmapFirstOrder* stream_bitmap_first);
	public unsafe delegate void pStreamBitmapNext(rdpContext* context, StreamBitmapNextOrder* stream_bitmap_next);
	public unsafe delegate void pDrawGdiPlusFirst(rdpContext* context, DrawGdiPlusFirstOrder* draw_gdiplus_first);
	public unsafe delegate void pDrawGdiPlusNext(rdpContext* context, DrawGdiPlusNextOrder* draw_gdiplus_next);
	public unsafe delegate void pDrawGdiPlusEnd(rdpContext* context, DrawGdiPlusEndOrder* draw_gdiplus_end);
	public unsafe delegate void pDrawGdiPlusCacheFirst(rdpContext* context, DrawGdiPlusCacheFirstOrder* draw_gdiplus_cache_first);
	public unsafe delegate void pDrawGdiPlusCacheNext(rdpContext* context, DrawGdiPlusCacheNextOrder* draw_gdiplus_cache_next);
	public unsafe delegate void pDrawGdiPlusCacheEnd(rdpContext* context, DrawGdiPlusCacheEndOrder* draw_gdiplus_cache_end);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpAltSecUpdate
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public IntPtr CreateOffscreenBitmap;
		public IntPtr SwitchSurface;
		public IntPtr CreateNineGridBitmap;
		public IntPtr FrameMarker;
		public IntPtr StreamBitmapFirst;
		public IntPtr StreamBitmapNext;
		public IntPtr DrawGdiPlusFirst;
		public IntPtr DrawGdiPlusNext;
		public IntPtr DrawGdiPlusEnd;
		public IntPtr DrawGdiPlusCacheFirst;
		public IntPtr DrawGdiPlusCacheNext;
		public IntPtr DrawGdiPlusCacheEnd;
		public fixed UInt32 paddingB[32-28];
	}
}

