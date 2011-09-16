using System;
using System.Runtime.InteropServices;

namespace Screenary
{
	public class FreeRDP
	{		
		private IntPtr rfx;
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern IntPtr rfx_context_new();
	
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern void rfx_context_free(IntPtr rfx);
		
		public FreeRDP()
		{
			rfx = rfx_context_new();
		}
		
		public void RfxProcessMessage(byte[] data, int length)
		{		
			Console.WriteLine("RfxProcessMessage: length:" + length);
		}
		
		~FreeRDP()
		{
			rfx_context_free(rfx);
		}
	}
}

