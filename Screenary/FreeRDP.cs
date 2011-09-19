using Gtk;
using Gdk;
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
			RFX_TILE* tile;
			byte[] buffer = new byte[4096 * 4];
			
			msg = rfx_process_message(rfx, data, (UInt32) length);
			ntiles = rfx_message_get_tile_count(msg);
			nrects = rfx_message_get_rect_count(msg);
			
			Console.WriteLine("ntiles:{0} nrects:{1}", ntiles, nrects);

			tile = rfx_message_get_tile(msg, 1);
			
			for (int index = 0; index < ntiles; index++)
			{
				tile = rfx_message_get_tile(msg, index);
				Marshal.Copy(new IntPtr(tile->data), buffer, 0, 4096 * 4);
				Cairo.ImageSurface surface = new Cairo.ImageSurface(buffer, Cairo.Format.ARGB32, 64, 64, 64 * 4);
				surface.WriteToPng(String.Format("/tmp/rfx/tile_{0:000}.png", index));
			}
			
			rfx_message_free(rfx, msg);
		}
		
		~FreeRDP()
		{
			rfx_context_free(rfx);
		}
	}
}

