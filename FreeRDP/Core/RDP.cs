using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	public unsafe class RDP
	{		
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
			Console.WriteLine("width:{0} height:{1} port:{2}",
				settings->width, settings->height, settings->port);
			
			settings->hostname = GetNativeAnsiString("localhost");
			settings->username = GetNativeAnsiString("Administrator");
			settings->password = GetNativeAnsiString("Password123!");
			
			return ((freerdp_connect(handle) == 0) ? false : true);
		}
		
		public bool Disconnect()
		{
			return ((freerdp_disconnect(handle) == 0) ? false : true);
		}
	}
}

