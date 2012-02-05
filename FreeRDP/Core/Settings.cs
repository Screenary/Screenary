using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpSettings
	{
		public freerdp* instance;
		public fixed UInt32 paddingA[16-1];
		
		/* Core Protocol Parameters */
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
		public UInt32 negotiationFlags;
		public fixed UInt32 paddingB[48-31];
		
		/* Connection Settings */
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
		public IntPtr passwordCookie;
		public fixed UInt32 paddingC[80-62];
		
		/* User Interface Parameters */
		public int softwareGdi;
		public int workarea;
		public int fullscreen;
		public int grabKeyboard;
		public int decorations;
		public UInt32 percentScreen;
		public int mouseMotion;
		public IntPtr windowTitle;
		public UInt64 parentWindowXid;
		public fixed UInt32 paddingD[112-89];
		
		/* Internal Parameters */
		public IntPtr homePath;
		public UInt32 shareId;
		public UInt32 pduSource;
		public IntPtr uniconv;
		public int serverMode;
		public IntPtr configPath;
		public IntPtr currentPath;
		public IntPtr developmentPath;
		public int developmentMode;
		public fixed UInt32 paddingE[144-121];
		
		/* Security */
		public int encryption;
		public int tlsSecurity;
		public int nlaSecurity;
		public int rdpSecurity;
		public UInt32 ntlmVersion;
		public int secureChecksum;
		public fixed UInt32 paddingF[160-150];
		
		/* Session */
		public int consoleAudio;
		public int consoleSession;
		public UInt32 redirectedSessionId;
		public fixed UInt32 paddingG[176-163];
	
		/* Output Control */
		public int refreshRect;
		public int suppressOutput;
		public int desktopResize;
		public fixed UInt32 paddingH[192-179];
	
		/* Reconnection */
		public int autoReconnection;
		public IntPtr clientAutoReconnectCookie;
		public IntPtr serverAutoReconnectCookie;
		public fixed UInt32 paddingI[208-195];
	
		/* Time Zone */
		public IntPtr clientTimeZone;
		public fixed UInt32 paddingJ[216-209];
	
		/* Capabilities */
		public UInt32 osMajorType;
		public UInt32 osMinorType;
		public UInt32 vcChunkSize;
		public int soundBeeps;
		public int smoothFonts;
		public int frameMarker;
		public int fastpathInput;
		public int fastpathOutput;
		public fixed byte receivedCaps[32];
		public fixed byte orderSupport[32];
		public int surfaceCommands;
		public int disableWallpaper;
		public int disableFullWindowDrag;
		public int disableMenuAnimations;
		public int disableTheming;
		public UInt32 connectionType;
		public UInt32 multifragMaxRequestSize;
		public fixed UInt32 paddingK[248-247];
	
		/* Certificate */
		public IntPtr certFile;
		public IntPtr privateKeyFile;
		public fixed byte clientHostname[32];
		public fixed byte clientProductId[32];
		public IntPtr serverRandom;
		public IntPtr serverCertificate;
		public int ignoreCertificate;
		public IntPtr serverCert;
		public IntPtr rdpKeyFile;
		public IntPtr serverKey;
		public IntPtr certificateName;
		public fixed UInt32 paddingL[280-273];
	
		/* Codecs */
		public int rfxCodec;
		public int nsCodec;
		public UInt32 rfxCodecId;
		public UInt32 nsCodecId;
		public UInt32 rfxCodecMode;
		public int frameAcknowledge;
		public fixed UInt32 paddingM[296-286];
	
		/* Recording */
		public int dumpRfx;
		public int playRfx;
		public IntPtr dumpRfxFile;
		public IntPtr playRfxFile;
		public fixed UInt32 paddingN[312-300];
	
		/* RemoteApp */
		public int remoteApp;
		public UInt32 numIconCaches;
		public UInt32 numIconCacheEntries;
		public int railLangbarSupported;
		public fixed UInt32 paddingO[320-316];
	
		/* Pointer */
		public int largePointer;
		public int colorPointer;
		public UInt32 pointerCacheSize;
		public fixed UInt32 paddingP[328-323];
	
		/* Bitmap Cache */
		public int bitmapCache;
		public int bitmapCacheV3;
		public int persistentBitmapCache;
		public UInt32 bitmapCacheV2NumCells;
		public IntPtr bitmapCacheV2CellInfo;
		public fixed UInt32 paddingQ[344-333];
	
		/* Offscreen Bitmap Cache */
		public int offscreenBitmapCache;
		public UInt32 offscreenBitmapCacheSize;
		public UInt32 offscreenBitmapCacheEntries;
		public fixed UInt32 paddingR[352-347];
	
		/* Glyph Cache */
		public int glyphCache;
		public UInt32 glyphSupportLevel;
		public IntPtr glyphCacheInfo;
		public IntPtr fragCacheInfo;
		public fixed UInt32 paddingS[360-356];
	
		/* Draw Nine Grid */
		public int drawNineGrid;
		public UInt32 drawNineGridCacheSize;
		public UInt32 drawNineGridCacheEntries;
		public fixed UInt32 paddingT[368-363];
	
		/* Draw GDI+ */
		public int drawGdiPlus;
		public int drawGdiPlusCache;
		public fixed UInt32 paddingU[376-370];
	
		/* Desktop Composition */
		public int desktopComposition;
		public fixed UInt32 paddingV[384-377];
	};
}

