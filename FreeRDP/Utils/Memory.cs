using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe class Memory
	{
		[DllImport("libfreerdp-utils")]
		public static extern IntPtr xmalloc(UIntPtr size);
		
		[DllImport("libfreerdp-utils")]
		public static extern IntPtr xzalloc(UIntPtr size);
		
		[DllImport("libfreerdp-utils")]
		public static extern IntPtr xrealloc(IntPtr ptr, UIntPtr size);
		
		[DllImport("libfreerdp-utils")]
		public static extern void xfree(IntPtr ptr);
		
		public Memory()
		{
		}
		
		public static IntPtr Malloc(int size)
		{		
			UIntPtr size_t = new UIntPtr((ulong) size);
			return xmalloc(size_t);
		}
		
		public static IntPtr Zalloc(int size)
		{
			UIntPtr size_t = new UIntPtr((ulong) size);
			return xzalloc(size_t);
		}
		
		public static IntPtr Realloc(IntPtr ptr, int size)
		{
			UIntPtr size_t = new UIntPtr((ulong) size);
			return xrealloc(ptr, size_t);
		}
		
		public static void Free(IntPtr ptr)
		{
			xfree(ptr);
		}
	}
}

