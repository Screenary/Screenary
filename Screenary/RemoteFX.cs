using System;
using System.Runtime.InteropServices;

namespace Screenary
{
	public unsafe class RemoteFX
	{
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
		
		private IntPtr rfx;
		private IntPtr msg;
		private UInt16 ntiles;
		private UInt16 nrects;
		private int itiles;
		private int irects;
		private byte[] buffer;
		private RFX_TILE* tile;
		private RFX_RECT* rect;
		
		[DllImport("libfreerdp-rfx")]
		public static extern IntPtr rfx_context_new();
	
		[DllImport("libfreerdp-rfx")]
		public static extern void rfx_context_free(IntPtr rfx);
		
		[DllImport("libfreerdp-rfx")]
		public static extern IntPtr rfx_process_message(IntPtr rfx, byte[] data, UInt32 length);
		
		[DllImport("libfreerdp-rfx")]
		public static extern UInt16 rfx_message_get_tile_count(IntPtr message);
		
		[DllImport("libfreerdp-rfx")]
		public static extern RFX_TILE* rfx_message_get_tile(IntPtr message, int index);
		
		[DllImport("libfreerdp-rfx")]
		public static extern UInt16 rfx_message_get_rect_count(IntPtr message);
		
		[DllImport("libfreerdp-rfx")]
		public static extern RFX_RECT* rfx_message_get_rect(IntPtr message, int index);
		
		[DllImport("libfreerdp-rfx")]
		public static extern void rfx_message_free(IntPtr rfx, IntPtr message);
		
		public RemoteFX()
		{
			itiles = 0;
			irects = 0;
			msg = IntPtr.Zero;
			rfx = rfx_context_new();
			buffer = new byte[4096 * 4];
		}
		
		public void Decode(byte[] data, int length)
		{	
			itiles = 0;
			irects = 0;
			
			if (msg != IntPtr.Zero)
					rfx_message_free(rfx, msg);
			
			msg = rfx_process_message(rfx, data, (UInt32) length);
			
			ntiles = rfx_message_get_tile_count(msg);
			nrects = rfx_message_get_rect_count(msg);
		}
		
		public bool HasNextTile()
		{
			return (itiles < ntiles);
		}
		
		public Cairo.ImageSurface GetNextTile()
		{
			Cairo.ImageSurface surface;
			
			tile = rfx_message_get_tile(msg, itiles++);
			Marshal.Copy(new IntPtr(tile->data), buffer, 0, 4096 * 4);
			surface = new Cairo.ImageSurface(buffer, Cairo.Format.ARGB32, 64, 64, 64 * 4);
			
			return surface;
		}
		
		public bool HasNextRect()
		{	
			return (irects < nrects);
		}
		
		public Cairo.Rectangle GetNextRect()
		{
			Cairo.Rectangle rectangle;
			
			rect = rfx_message_get_rect(msg, irects++);
			
			rectangle = new Cairo.Rectangle((double) rect->x, (double) rect->y,
				(double) rect->width, (double) rect->height);
			
			return rectangle;
		}
		
		~RemoteFX()
		{
			if (msg != IntPtr.Zero)
					rfx_message_free(rfx, msg);
			
			rfx_context_free(rfx);
		}
	}
}

