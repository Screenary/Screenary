using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pBitmap_New(rdpContext* context, rdpBitmap* bitmap);
	public unsafe delegate void pBitmap_Free(rdpContext* context, rdpBitmap* bitmap);
	public unsafe delegate void pBitmap_Paint(rdpContext* context, rdpBitmap* bitmap);
	public unsafe delegate void pBitmap_Decompress(rdpContext* context, rdpBitmap* bitmap);
	public unsafe delegate void pBitmap_SetSurface(rdpContext* context, rdpBitmap* bitmap);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpBitmap
	{
		public IntPtr size;
		public IntPtr New;
		public IntPtr Free;
		public IntPtr Paint;
		public IntPtr Decompress;
		public IntPtr SetSurface;
		public fixed UInt32 paddingA[16-6];
		
		public UInt32 left;
		public UInt32 top;
		public UInt32 right;
		public UInt32 bottom;
		public UInt32 width;
		public UInt32 height;
		public UInt32 bpp;
		public UInt32 flags;
		public UInt32 length;
		public IntPtr data;
		public fixed UInt32 paddingB[32-26];
		
		public UInt32 compressed;
		public UInt32 ephemeral;
		public fixed UInt32 paddingC[64-34];
	}
}

