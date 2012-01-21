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
		public static extern int freerdp_check_fds(freerdp* instance);
		
		[DllImport("libfreerdp-core")]
		public static extern freerdp* freerdp_new();
		
		[DllImport("libfreerdp-core")]
		public static extern void freerdp_free(freerdp* instance);
		
		public int Port { get { return (int) settings->port; } set { settings->port = (UInt32) value; } }
		public int Width { get { return (int) settings->width; } set { settings->width = (UInt32) value; } }
		public int Height { get { return (int) settings->height; } set { settings->height = (UInt32) value; } }
		
		private freerdp* handle;
		private rdpContext* context;
		private rdpSettings* settings;
		
		private IUpdate iUpdate;
		private IPrimaryUpdate iPrimaryUpdate;
		private ISecondaryUpdate iSecondaryUpdate;
		private IAltSecUpdate iAltSecUpdate;
		
		private pContextNew hContextNew;
		private pContextFree hContextFree;
		
		private pPreConnect hPreConnect;
		private pPostConnect hPostConnect;
		
		private Update update;
		private PrimaryUpdate primary;
		
		public RDP()
		{
			handle = freerdp_new();
			
			iUpdate = null;
			iPrimaryUpdate = null;
			iSecondaryUpdate = null;
			iAltSecUpdate = null;
			
			hContextNew = new pContextNew(ContextNew);
			hContextFree = new pContextFree(ContextFree);
			
			handle->ContextNew = Marshal.GetFunctionPointerForDelegate(hContextNew);
			handle->ContextFree = Marshal.GetFunctionPointerForDelegate(hContextFree);

			freerdp_context_new(handle);
		}
		
		~RDP()
		{

		}
		
		public void SetUpdateInterface(IUpdate iUpdate)
		{
			this.iUpdate = iUpdate;
		}
		
		public void SetPrimaryUpdateInterface(IPrimaryUpdate iPrimaryUpdate)
		{
			this.iPrimaryUpdate = iPrimaryUpdate;
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
			
			hPreConnect = new pPreConnect(this.PreConnect);
			hPostConnect = new pPostConnect(this.PostConnect);
			
			instance->PreConnect = Marshal.GetFunctionPointerForDelegate(hPreConnect);
			instance->PostConnect = Marshal.GetFunctionPointerForDelegate(hPostConnect);
			
			this.context = context;
			settings = instance->settings;
		}
		
		void ContextFree(freerdp* instance, rdpContext* context)
		{
			Console.WriteLine("ContextFree");
		}
		
		bool PreConnect(freerdp* instance)
		{
			Console.WriteLine("PreConnect");
			
			if (iUpdate != null)
			{
				update = new Update(instance->context);
				update.RegisterInterface(iUpdate);
			}
			
			if (iPrimaryUpdate != null)
			{
				primary = new PrimaryUpdate(instance->context);
				primary.RegisterInterface(iPrimaryUpdate);
			}
			
			settings->rfxCodec = 1;
			settings->fastpathOutput = 1;
			settings->colorDepth = 32;
			settings->frameAcknowledge = 0;
			settings->performanceFlags = 0;
			settings->largePointer = 1;
			
			settings->glyphCache = 0;
			settings->bitmapCache = 0;
			settings->offscreenBitmapCache = 0;
			
			return true;
		}
		
		bool PostConnect(freerdp* instance)
		{
			Console.WriteLine("PostConnect");
			return true;
		}
		
		public bool Connect(string hostname, int port, string username, string domain, string password)
		{			
			Console.WriteLine("width:{0} height:{1} port:{2}",
				settings->width, settings->height, settings->port);
			
			settings->hostname = GetNativeAnsiString(hostname);
			settings->username = GetNativeAnsiString(username);
			
			if (domain.Length > 1)
				settings->domain = GetNativeAnsiString(domain);
			
			if (password.Length > 1)
				settings->password = GetNativeAnsiString(password);
			else
				settings->authentication = 0;
			
			return ((freerdp_connect(handle) == 0) ? false : true);
		}
		
		public bool Disconnect()
		{
			return ((freerdp_disconnect(handle) == 0) ? false : true);
		}
		
		public bool CheckFileDescriptor()
		{
			return ((freerdp_check_fds(handle) == 0) ? false : true);
		}
	}
}

