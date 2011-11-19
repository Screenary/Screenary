using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	public unsafe class Client
	{
		[StructLayout(LayoutKind.Explicit,Size=256)]
		public class rdpContext
		{
			[FieldOffset(0)] public IntPtr instance;
			[FieldOffset(4)] public IntPtr peer;
			
			[FieldOffset(64)] public IntPtr rdp;
			[FieldOffset(68)] public IntPtr gdi;
			[FieldOffset(72)] public IntPtr rail;
			[FieldOffset(76)] public IntPtr cache;
			[FieldOffset(80)] public IntPtr channels;
			[FieldOffset(84)] public IntPtr graphics;
		};
		
		[StructLayout(LayoutKind.Explicit,Size=320)]
		public unsafe class freerdp
		{
			[FieldOffset(0)] public IntPtr context;
			
			[FieldOffset(64)] public IntPtr rdpInput;
			[FieldOffset(68)] public IntPtr rdpUpdate;
			[FieldOffset(72)] public IntPtr rdpSettings;
			
			[FieldOffset(128)] public Int32 ContextSize;
			[FieldOffset(132)] public pContextNew ContextNew;
			[FieldOffset(136)] public pContextFree ContextFree;
			
			[FieldOffset(192)] public pPreConnect PreConnect;
			[FieldOffset(196)] public pPostConnect PostConnect;
			[FieldOffset(200)] public IntPtr Authenticate;
			[FieldOffset(204)] public IntPtr VerifyAuthenticate;
			
			[FieldOffset(256)] public IntPtr SendChannelData;
			[FieldOffset(260)] public IntPtr RecvChannelData;
		};
		
		public delegate void pContextNew(freerdp instance, rdpContext context);
		public delegate void pContextFree(freerdp instance, rdpContext context);
		
		public delegate bool pPreConnect(freerdp instance);
		public delegate bool pPostConnect(freerdp instance);
		
		private freerdp instance;
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_context_new(freerdp instance);
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_context_free(freerdp instance);
		
		[DllImport("libfreerdp-core")]
		public static extern bool freerdp_connect(freerdp instance);
		
		[DllImport("libfreerdp-core")]
		public static extern bool freerdp_disconnect(freerdp instance);
		
		[DllImport("libfreerdp-core")]
		public static extern freerdp freerdp_new();
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_free(freerdp instance);
		
		public Client()
		{
			instance = freerdp_new();
		}
		
		~Client()
		{

		}
		
		void ContextNew(freerdp instance, rdpContext context)
		{

		}
		
		void ContextFree(freerdp instance, rdpContext context)
		{
		}
	}
}

