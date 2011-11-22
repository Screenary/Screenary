using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpCache
	{
		public rdpGlyphCache* glyph;
		public rdpBrushCache* brush;
		public rdpPointerCache* pointer;
		public rdpBitmapCache* bitmap;
		public rdpOffscreenCache* offscreen;
		public rdpPaletteCache* palette;
	}
}

