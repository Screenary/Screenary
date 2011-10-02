using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe class Rfx
	{
		protected IntPtr handle;
		
		public enum RFX_PIXEL_FORMAT
		{
			BGRA = 0,
			RGBA = 1,
			BGR = 2,
			RGB = 3,
			BGR565_LE = 4,
			RGB565_LE = 5,
			PALETTE4_PLANER = 6,
			PALETTE8 = 7
		}
		
		[DllImport("libfreerdp-rfx")]
		public static extern IntPtr rfx_context_new();
	
		[DllImport("libfreerdp-rfx")]
		public static extern void rfx_context_free(IntPtr handle);
		
		[DllImport("libfreerdp-rfx")]
		public static extern void rfx_context_set_pixel_format(IntPtr handle, RFX_PIXEL_FORMAT format);
		
		public Rfx()
		{
			handle = rfx_context_new();
			rfx_context_set_pixel_format(handle, RFX_PIXEL_FORMAT.RGBA);
		}
		
		public RfxMessage ParseMessage(byte[] data, UInt32 length)
		{
			return RfxMessage.Parse(handle, data, length);
		}
		
		~Rfx()
		{
			rfx_context_free(handle);
		}
	}
}

