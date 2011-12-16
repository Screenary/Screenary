using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheBitmapOrder
	{
		public UInt32 cacheId;
		public UInt32 bitmapBpp;
		public UInt32 bitmapWidth;
		public UInt32 bitmapHeight;
		public UInt32 bitmapLength;
		public UInt32 cacheIndex;
		public fixed byte bitmapComprHdr[8];
		public byte* bitmapDataStream;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheBitmapV2Order
	{
		public UInt32 cacheId;
		public UInt32 flags;
		public UInt32 key1;
		public UInt32 key2;
		public UInt32 bitmapBpp;
		public UInt32 bitmapWidth;
		public UInt32 bitmapHeight;
		public UInt32 bitmapLength;
		public UInt32 cacheIndex;
		public int compressed;
		public fixed byte bitmapComprHdr[8];
		public byte* bitmapDataStream;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct BitmapDataEx
	{
		public UInt32 bpp;
		public UInt32 codecID;
		public UInt32 width;
		public UInt32 height;
		public UInt32 length;
		public byte* data;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheBitmapV3Order
	{
		public UInt32 cacheId;
		public UInt32 bpp;
		public UInt32 flags;
		public UInt32 cacheIndex;
		public UInt32 key1;
		public UInt32 key2;
		public BitmapDataEx* bitmapData;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheColorTableOrder
	{
		public UInt32 cacheIndex;
		public UInt32 numberColors;
		public UInt32* colorTable;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GlyphData
	{
		public UInt32 cacheIndex;
		public Int32 x;
		public Int32 y;
		public UInt32 cx;
		public UInt32 cy;
		public UInt32 cb;
		public byte* aj;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheGlyphOrder
	{
		public UInt32 cacheId;
		public UInt32 cGlyphs;
		//public GlyphData glyphData[255];
		public byte* unicodeCharacters;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GlyphDataV2
	{
		public UInt32 cacheIndex;
		public Int32 x;
		public Int32 y;
		public UInt32 cx;
		public UInt32 cy;
		public UInt32 cb;
		public byte* aj;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheGlyphV2Order
	{
		public UInt32 cacheId;
		public UInt32 flags;
		public UInt32 cGlyphs;
		//public GlyphDataV2 glyphData[255];
		public byte* unicodeCharacters;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CacheBrushOrder
	{
		public UInt32 index;
		public UInt32 bpp;
		public UInt32 cx;
		public UInt32 cy;
		public UInt32 style;
		public UInt32 length;
		public byte* data;
	}
	
	public unsafe delegate void pCacheBitmap(rdpContext* context, CacheBitmapOrder* cache_bitmap_order);
	public unsafe delegate void pCacheBitmapV2(rdpContext* context, CacheBitmapV2Order* cache_bitmap_v2_order);
	public unsafe delegate void pCacheBitmapV3(rdpContext* context, CacheBitmapV3Order* cache_bitmap_v3_order);
	public unsafe delegate void pCacheColorTable(rdpContext* context, CacheColorTableOrder* cache_color_table_order);
	public unsafe delegate void pCacheGlyph(rdpContext* context, CacheGlyphOrder* cache_glyph_order);
	public unsafe delegate void pCacheGlyphV2(rdpContext* context, CacheGlyphV2Order* cache_glyph_v2_order);
	public unsafe delegate void pCacheBrush(rdpContext* context, CacheBrushOrder* cache_brush_order);
	
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

