using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpPointer
	{
		public IntPtr size;
		public IntPtr New;
		public IntPtr Free;
		public IntPtr Set;
		public fixed UInt32 paddingA[16-4];
		
		public UInt32 xPos;
		public UInt32 yPos;
		public UInt32 width;
		public UInt32 height;
		public UInt32 xorBpp;
		public UInt32 lengthAndMask;
		public UInt32 lengthXorMask;
		public IntPtr xorMaskData;
		public IntPtr andMaskData;
		public fixed UInt32 paddingB[32-25];
	}
}
