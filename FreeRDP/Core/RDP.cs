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
			public fixed UInt32 paddingA[16-2];
			
			public int argc;
			public IntPtr argv;
			public fixed UInt32 paddingB[32-18];
			
			public IntPtr rdp;
			public IntPtr gdi;
			public IntPtr rail;
			public IntPtr cache;
			public IntPtr channels;
			public IntPtr graphics;
			public fixed UInt32 paddingC[64-38];
		};
		
		[StructLayout(LayoutKind.Sequential)]
		public struct rdpSettings
		{
			public IntPtr instance;
			public fixed UInt32 paddingA[16-1];
			
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
			public fixed UInt32 paddingB[48-30];
			
			public UInt32 port;
			public int ipv6;
			public IntPtr hostname;
			public IntPtr username;
			public IntPtr password;
			public IntPtr domain;
			public IntPtr shell;
			public IntPtr directory;
			public IntPtr ipAddress;
			public IntPtr clientDir;
			public int autologon;
			public int compression;
			public UInt32 performanceFlags;
			public fixed UInt32 paddingC[80-61];
			
			public fixed UInt32 paddingD[112-80];
			
			public IntPtr homePath;
			public UInt32 shareId;
			public UInt32 pduSource;
			public IntPtr uniconv;
			public int serverMode;
			public fixed UInt32 paddingE[144-117];
			
			public int encryption;
			public int tlsSecurity;
			public int nlaSecurity;
			public int rdpSecurity;
			public fixed UInt32 paddingF[160-148];
		};
		
		[StructLayout(LayoutKind.Sequential)]
		public struct freerdp
		{
			public IntPtr context;
			public fixed UInt32 paddingA[16-1];
			
			public IntPtr input;
			public IntPtr update;
			public rdpSettings* settings;
			public fixed UInt32 paddingB[32-19];
			
			public UIntPtr ContextSize;
			public IntPtr ContextNew;
			public IntPtr ContextFree;
			public fixed UInt32 paddingC[48-35];
			
			public IntPtr PreConnect;
			public IntPtr PostConnect;
			public IntPtr Authenticate;
			public IntPtr VerifyAuthenticate;
			public fixed UInt32 paddingD[64-52];
			
			public IntPtr SendChannelData;
			public IntPtr RecvChannelData;
			public fixed UInt32 paddingF[80-66];
		};
		
		public delegate void pContextNew(freerdp* instance, rdpContext* context);
		public delegate void pContextFree(freerdp* instance, rdpContext* context);
		
		public delegate bool pPreConnect(freerdp* instance);
		public delegate bool pPostConnect(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_context_new(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_context_free(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern int freerdp_connect(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern int freerdp_disconnect(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern freerdp* freerdp_new();
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_free(freerdp* instance);
		
		private freerdp* handle;
		private rdpSettings* settings;
		
		private pContextNew hContextNew;
		private pContextFree hContextFree;
		
		private pPreConnect hPreConnect;
		private pPostConnect hPostConnect;
		
		public RDP()
		{
			handle = freerdp_new();
			
			hContextNew = new pContextNew(ContextNew);
			hContextFree = new pContextFree(ContextFree);
			
			handle->ContextNew = Marshal.GetFunctionPointerForDelegate(hContextNew);
			handle->ContextFree = Marshal.GetFunctionPointerForDelegate(hContextFree);

			freerdp_context_new(handle);
		}
		
		~RDP()
		{

		}
		
		private IntPtr GetNativeAnsiString(string str)
		{
			ASCIIEncoding strEncoder = new ASCIIEncoding();
			
			int size = strEncoder.GetByteCount(str);
			IntPtr pStr = Memory.Zalloc(size);
			byte[] buffer = strEncoder.GetBytes(str);
			Marshal.Copy(buffer, 0, pStr, size);
			
			return pStr;
		}
		
		void ContextNew(freerdp* instance, rdpContext* context)
		{
			Console.WriteLine("ContextNew");
			
			hPreConnect = new pPreConnect(PreConnect);
			hPostConnect = new pPostConnect(PostConnect);
			
			instance->PreConnect = Marshal.GetFunctionPointerForDelegate(hPreConnect);
			instance->PostConnect = Marshal.GetFunctionPointerForDelegate(hPostConnect);
			
			settings = instance->settings;
			
			Console.WriteLine("width:{0} height:{1} port:{2}",
				settings->width, settings->height, settings->port);
			
			settings->hostname = GetNativeAnsiString("localhost");
			settings->username = GetNativeAnsiString("Administrator");
			settings->password = GetNativeAnsiString("Password123!");
			
			freerdp_connect(instance);
		}
		
		void ContextFree(freerdp* instance, rdpContext* context)
		{
			Console.WriteLine("ContextFree");
		}
		
		bool PreConnect(freerdp* instance)
		{
			Console.WriteLine("PreConnect");
			
			return true;
		}
		
		bool PostConnect(freerdp* instance)
		{
			Console.WriteLine("PostConnect");
			return true;
		}
		
		public bool Connect()
		{
			return ((freerdp_connect(handle) == 0) ? false : true);
		}
		
		public bool Disconnect()
		{
			return ((freerdp_disconnect(handle) == 0) ? false : true);
		}
	}
}

