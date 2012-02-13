using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pContextNew(freerdp* instance, rdpContext* context);
	public unsafe delegate void pContextFree(freerdp* instance, rdpContext* context);
		
	public unsafe delegate bool pPreConnect(freerdp* instance);
	public unsafe delegate bool pPostConnect(freerdp* instance);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct freerdp
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public rdpInput* input;
		public rdpUpdate* update;
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
}

