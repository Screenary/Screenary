using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
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
