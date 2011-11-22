using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pCreateOffscreenBitmap(rdpContext* context, IntPtr create_offscreen_bitmap);
	public unsafe delegate void pSwitchSurface(rdpContext* context, IntPtr switch_surface);
	public unsafe delegate void pCreateNineGridBitmap(rdpContext* context, IntPtr* create_nine_grid_bitmap);
	public unsafe delegate void pFrameMarker(rdpContext* context, IntPtr frame_marker);
	public unsafe delegate void pStreamBitmapFirst(rdpContext* context, IntPtr stream_bitmap_first);
	public unsafe delegate void pStreamBitmapNext(rdpContext* context, IntPtr stream_bitmap_next);
	public unsafe delegate void pDrawGdiPlusFirst(rdpContext* context, IntPtr draw_gdiplus_first);
	public unsafe delegate void pDrawGdiPlusNext(rdpContext* context, IntPtr draw_gdiplus_next);
	public unsafe delegate void pDrawGdiPlusEnd(rdpContext* context, IntPtr draw_gdiplus_end);
	public unsafe delegate void pDrawGdiPlusCacheFirst(rdpContext* context, IntPtr draw_gdiplus_cache_first);
	public unsafe delegate void pDrawGdiPlusCacheNext(rdpContext* context, IntPtr draw_gdiplus_cache_next);
	public unsafe delegate void pDrawGdiPlusCacheEnd(rdpContext* context, IntPtr draw_gdiplus_cache_end);
	
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

