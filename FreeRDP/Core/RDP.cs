using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	public unsafe class RDP
	{
		[StructLayout(LayoutKind.Explicit,Size=256)]
		public class rdpContext
		{
			[FieldOffset(0)] public IntPtr instance;
			[FieldOffset(8)] public IntPtr peer;
			
			[FieldOffset(64)] public IntPtr rdp;
			[FieldOffset(72)] public IntPtr gdi;
			[FieldOffset(80)] public IntPtr rail;
			[FieldOffset(88)] public IntPtr cache;
			[FieldOffset(96)] public IntPtr channels;
			[FieldOffset(104)] public IntPtr graphics;
		};
		
		[StructLayout(LayoutKind.Explicit)]
		public unsafe class rdpSettings
		{
			[FieldOffset(0)] public IntPtr instance;
			
			[FieldOffset(64)] public UInt32 width;
			[FieldOffset(68)] public UInt32 height;
			[FieldOffset(72)] public UInt32 rdpVersion;
			[FieldOffset(76)] public UInt32 colorDepth;
			[FieldOffset(80)] public UInt32 kbdLayout;
			[FieldOffset(84)] public UInt32 kbdType;
			[FieldOffset(88)] public UInt32 kbdSubType;
			[FieldOffset(92)] public UInt32 kbdFnKeys;
			[FieldOffset(96)] public UInt32 clientBuild;
			[FieldOffset(100)] public UInt32 requestedProtocols;
			[FieldOffset(104)] public UInt32 selectedProtocol;
			[FieldOffset(108)] public UInt32 encryptionMethod;
			[FieldOffset(112)] public UInt32 encryptionLevel;
			[FieldOffset(116)] public int authentication;
			
			[FieldOffset(192)] public UInt32 port;
			[FieldOffset(200)] public int ipv6;
			//[FieldOffset(204), MarshalAs(UnmanagedType.LPStr)] public string hostname;
			/*
			[FieldOffset(212)] public string username;
			[FieldOffset(220)] public string password;
			[FieldOffset(228)] public string domain;
			[FieldOffset(236)] public string shell;
			[FieldOffset(244)] public string directory;
			[FieldOffset(252)] public string ipAddress;
			[FieldOffset(260)] public string clientDir;
			*/
			[FieldOffset(268)] public int autologon;
			[FieldOffset(276)] public int compression;
		};
		
		[StructLayout(LayoutKind.Explicit,Size=320)]
		public unsafe class freerdp
		{
			[FieldOffset(0)] public IntPtr context;
			
			[FieldOffset(64)] public IntPtr input;
			[FieldOffset(72)] public IntPtr update;
			[FieldOffset(80)] public rdpSettings settings;
			
			[FieldOffset(128)] public int ContextSize;
			[FieldOffset(136)] public pContextNew ContextNew;
			[FieldOffset(144)] public pContextFree ContextFree;
			
			[FieldOffset(192)] public pPreConnect PreConnect;
			[FieldOffset(200)] public pPostConnect PostConnect;
			[FieldOffset(208)] public IntPtr Authenticate;
			[FieldOffset(216)] public IntPtr VerifyAuthenticate;
			
			[FieldOffset(256)] public IntPtr SendChannelData;
			[FieldOffset(264)] public IntPtr RecvChannelData;
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
			Console.WriteLine("ContextSize: {0}", instance.ContextSize);
			
			instance.ContextNew = ContextNew;
			instance.ContextFree = ContextFree;
			freerdp_context_new(instance);
			
			settings = instance.settings;
			
			settings.width = 1024;
			settings.height = 768;
			settings.autologon = 1;
			settings.colorDepth = 16;
			//settings.hostname = "localhost";
			//settings.username = "Administrator";
			//settings.password = "Password";
			
			instance.PreConnect = PreConnect;
			instance.PostConnect = PostConnect;
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

