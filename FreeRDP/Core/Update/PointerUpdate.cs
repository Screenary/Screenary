using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pPointerPosition(rdpContext* context, IntPtr pointerPosition);
	public unsafe delegate void pPointerSystem(rdpContext* context, IntPtr pointerSystem);
	public unsafe delegate void pPointerColor(rdpContext* context, IntPtr pointerColor);
	public unsafe delegate void pPointerNew(rdpContext* context, IntPtr pointerNew);
	public unsafe delegate void pPointerCached(rdpContext* context, IntPtr pointerCached);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpPointerUpdate
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public IntPtr PointerPosition;
		public IntPtr PointerSystem;
		public IntPtr PointerColor;
		public IntPtr PointerNew;
		public IntPtr PointerCached;
		public fixed UInt32 paddingB[32-21];
	}
}

