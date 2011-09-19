using System;
using System.Runtime.InteropServices;

namespace Screenary
{
	public unsafe class FreeRDP
	{		
		private IntPtr rfx;
		
		[StructLayout(LayoutKind.Sequential)]
		public struct RFX_RECT
		{
			public UInt16 x;
			public UInt16 y;
			public UInt16 width;
			public UInt16 height;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct RFX_TILE
		{
			public UInt16 x;
			public UInt16 y;
			public byte* data;
		}
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern IntPtr rfx_context_new();
	
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern void rfx_context_free(IntPtr rfx);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern IntPtr rfx_process_message(IntPtr rfx, byte[] data, UInt32 length);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern UInt16 rfx_message_get_tile_count(IntPtr message);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern RFX_TILE* rfx_message_get_tile(IntPtr message, int index);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern UInt16 rfx_message_get_rect_count(IntPtr message);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern RFX_RECT* rfx_message_get_rect(IntPtr message, int index);
		
		[DllImport("/usr/local/lib/libfreerdp-rfx.so")]
		public static extern void rfx_message_free(IntPtr rfx, IntPtr message);
		
		public FreeRDP()
		{
			rfx = rfx_context_new();
		}
		
		public void RfxProcessMessage(byte[] data, int length)
		{	
			IntPtr msg;
			UInt16 ntiles;
			UInt16 nrects;
			RFX_RECT* rect;
			RFX_TILE* tile;
		
			Console.WriteLine("length:" + length);
			
			msg = rfx_process_message(rfx, data, (UInt32) length);
			ntiles = rfx_message_get_tile_count(msg);
			nrects = rfx_message_get_rect_count(msg);
			
			Console.WriteLine("ntiles:{0} nrects:{1}", ntiles, nrects);

			rect = rfx_message_get_rect(msg, 1);
			Console.WriteLine("rect: x:{0} y:{1} w:{2} h:{3}", rect->x, rect->y, rect->width, rect->height);
			
			tile = rfx_message_get_tile(msg, 1);
			Console.WriteLine("tile: x:{0} y:{1}", tile->x, tile->y);
			
			rfx_message_free(rfx, msg);
		}
		
		~FreeRDP()
		{
			rfx_context_free(rfx);
		}
	}
}

