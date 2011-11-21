using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpContext
	{
		public IntPtr instance;
		public IntPtr peer;
		public fixed UInt32 paddingA[16-2];
		
		public int argc;
		public IntPtr argv;
		public fixed UInt32 paddingB[32-18];
		
		public IntPtr rdp;
		public IntPtr gdi;
		public IntPtr rail;
		public IntPtr cache;
		public IntPtr channels;
		public IntPtr graphics;
		public fixed UInt32 paddingC[64-38];
	};
}

