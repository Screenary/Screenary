using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pCacheBitmap(rdpContext* context, IntPtr cache_bitmap_order);
	public unsafe delegate void pCacheBitmapV2(rdpContext* context, IntPtr cache_bitmap_v2_order);
	public unsafe delegate void pCacheBitmapV3(rdpContext* context, IntPtr cache_bitmap_v3_order);
	public unsafe delegate void pCacheColorTable(rdpContext* context, IntPtr cache_color_table_order);
	public unsafe delegate void pCacheGlyph(rdpContext* context, IntPtr cache_glyph_order);
	public unsafe delegate void pCacheGlyphV2(rdpContext* context, IntPtr cache_glyph_v2_order);
	public unsafe delegate void pCacheBrush(rdpContext* context, IntPtr cache_brush_order);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpSecondaryUpdate
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public IntPtr CacheBitmap;
		public IntPtr CacheBitmapV2;
		public IntPtr CacheBitmapV3;
		public IntPtr CacheColorTable;
		public IntPtr CacheGlyph;
		public IntPtr CacheGlyphV2;
		public IntPtr CacheBrush;
		public fixed UInt32 paddingB[32-23];
	}
}

