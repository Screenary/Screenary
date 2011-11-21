using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpSettings
	{
		public freerdp* instance;
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
}

