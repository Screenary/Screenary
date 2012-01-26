using System;
using FreeRDP;
using System.Threading;

namespace Screenary.Client
{
	public unsafe class RdpSource : IUpdate
	{
		private RDP rdp;
		private int port;
		private string hostname;
		private string username;
		private string domain;
		private string password;
		
		private Thread thread;
		private ISource iSource;
		
		public RdpSource(ISource iSource)
		{
			port = 3389;
			hostname = "localhost";
			username = "Administrator";
			domain = "";
			password = "";
			rdp = new RDP();
			this.iSource = iSource;
			thread = new Thread(() => ThreadProc(rdp));
		}
		
		public void Connect(string hostname, int port, string username, string domain, string password)
		{
			rdp.SetUpdateInterface(this);			
			rdp.Connect(hostname, port, username, domain, password);
			thread.Start();
		}
		
		public void BeginPaint(rdpContext* context) { }
		public void EndPaint(rdpContext* context) { }
		public void SetBounds(rdpContext* context, rdpBounds* bounds) { }
		public void Synchronize(rdpContext* context) { }
		public void DesktopResize(rdpContext* context) { }
		public void BitmapUpdate(rdpContext* context, BitmapUpdate* bitmap) { }
		public void Palette(rdpContext* context, PaletteUpdate* palette) { }
		public void PlaySound(rdpContext* context, PlaySoundUpdate* playSound) { }
		
		public void SurfaceBits(rdpContext* context, SurfaceBits* surfaceBits)
		{			
			SurfaceBitsCommand cmd = new SurfaceBitsCommand();
			cmd.Read(surfaceBits);
			iSource.OnSurfaceCommand(cmd);
		}
		
		static void ThreadProc(RDP rdp)
		{
			while (true)
			{
				rdp.CheckFileDescriptor();
				Thread.Sleep(10);
			}
		}
	}
}

