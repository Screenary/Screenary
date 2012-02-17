using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpInput
	{
		public rdpContext* context;
		public IntPtr param;
		public fixed UInt32 paddingA[16-2];
		
		public IntPtr SynchronizeEvent;
		public IntPtr KeyboardEvent;
		public IntPtr UnicodeKeyboardEvent;
		public IntPtr MouseEvent;
		public IntPtr ExtendedMouseEvent;
		public fixed UInt32 paddingB[32-21];
	};
}

