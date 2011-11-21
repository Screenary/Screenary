using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe class GDI
	{
		[DllImport("libfreerdp-gdi")]
		public static extern int gdi_init(freerdp* instance, UInt32 flags, IntPtr buffer);
		
		[DllImport("libfreerdp-gdi")]
		public static extern void gdi_free(freerdp* instance);
	
		private UInt32 flags;
		private freerdp* instance;
		
		public GDI(freerdp* instance)
		{
			this.flags = 0;
			this.instance = instance;
		}
		
		public bool Init()
		{
			return ((gdi_init(instance, flags, IntPtr.Zero) == 0) ? false : true);
		}
		
		~GDI()
		{
				gdi_free(instance);
		}
	}
}

