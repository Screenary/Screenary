using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	public unsafe class RDP
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct rdpContext
		{
			public IntPtr instance;
			public IntPtr peer;
			public fixed UInt32 paddingA[16 - 2];
			
			public int argc;
			public IntPtr argv;
			public fixed UInt32 paddingB[32 - 18];
			
			public IntPtr rdp;
			public IntPtr gdi;
			public IntPtr rail;
			public IntPtr cache;
			public IntPtr channels;
			public IntPtr graphics;
			public fixed UInt32 paddingC[64 - 38];
		};
		
		[StructLayout(LayoutKind.Sequential)]
		public struct rdpSettings
		{
			public IntPtr instance;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(16-1))] public UInt32[] paddingA;
			
			public UInt32 width;
			public UInt32 height;
			public UInt32 rdpVersion;
			public UInt32 colorDepth;
			public UInt32 kbdLayout;
			public UInt32 kbdType;
			public UInt32 kbdSubType;
			public UInt32 kbdFnKeys;
			public UInt32 clientBuild;
			public UInt32 requestedProtocols;
			public UInt32 selectedProtocol;
			public UInt32 encryptionMethod;
			public UInt32 encryptionLevel;
			public int authentication;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(48-30))] public UInt32[] paddingB;
			
			public UInt32 port;
			public int ipv6;
		};
		
		[StructLayout(LayoutKind.Sequential)]
		public struct freerdp
		{
			public IntPtr context;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(16-1))] public UInt32[] paddingA;
			
			public IntPtr input;
			public IntPtr update;
			public IntPtr settings;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(32-19))] public UInt32[] paddingB;
			
			public UIntPtr ContextSize;
			public pContextNew ContextNew;
			public pContextFree ContextFree;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(48-35))] public UInt32[] paddingC;
			
			public pPreConnect PreConnect;
			public pPostConnect PostConnect;
			public IntPtr Authenticate;
			public IntPtr VerifyAuthenticate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(64-52))] public UInt32[] paddingD;
			
			public IntPtr SendChannelData;
			public IntPtr RecvChannelData;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=(80-66))] public UInt32[] paddingF;
		};
		
		public delegate void pContextNew(freerdp instance, rdpContext context);
		public delegate void pContextFree(freerdp instance, rdpContext context);
		
		public delegate bool pPreConnect(freerdp instance);
		public delegate bool pPostConnect(freerdp instance);
		
		private freerdp instance;
		private rdpSettings settings;
		
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
		
		public RDP()
		{
			instance = freerdp_new();
			
			instance.ContextNew = ContextNew;
			instance.ContextFree = ContextFree;
			freerdp_context_new(instance);
			
			/*
			settings = instance.settings;
			
			Console.WriteLine("width:{0} height:{1} port: {2}",
				settings.width, settings.height, settings.port);
			
			settings.width = 1024;
			settings.height = 768;
			settings.colorDepth = 16;			
			//settings.hostname = "localhost";
			//settings.username = "Administrator";
			//settings.password = "Password";
			
			instance.PreConnect = PreConnect;
			instance.PostConnect = PostConnect;
			*/
		}
		
		~RDP()
		{

		}
		
		void ContextNew(freerdp instance, rdpContext context)
		{
			Console.WriteLine("ContextNew");
		}
		
		void ContextFree(freerdp instance, rdpContext context)
		{
			
		}
		
		bool PreConnect(freerdp instance)
		{
			Console.WriteLine("PreConnect");
			
			return true;
		}
		
		bool PostConnect(freerdp instance)
		{
			Console.WriteLine("PostConnect");
			return true;
		}
		
		public bool Connect()
		{
			return freerdp_connect(instance);
		}
		
		public bool Disconnect()
		{
			return freerdp_disconnect(instance);
		}
	}
}

