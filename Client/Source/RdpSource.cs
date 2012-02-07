using System;
using FreeRDP;
using System.Threading;

namespace Screenary.Client
{
	public unsafe class RdpSource : IUpdate, IPrimaryUpdate
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
			rdp.SetPrimaryUpdateInterface(this);
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
		
		public void DstBlt(rdpContext* context, DstBltOrder* dstblt) { }
		public void PatBlt(rdpContext* context, PatBltOrder* patblt) { }
		public void ScrBlt(rdpContext* context, ScrBltOrder* scrblt) { }
		public void OpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect) { }
		public void DrawNineGrid(rdpContext* context, DrawNineGridOrder* drawNineGrid) { }	
		public void MultiDstBlt(rdpContext* context, MultiDstBltOrder* multi_dstblt) { }
		public void MultiPatBlt(rdpContext* context, MultiPatBltOrder* multi_patblt) { }		
		public void MultiScrBlt(rdpContext* context, MultiScrBltOrder* multi_scrblt) { }
		public void MultiOpaqueRect(rdpContext* context, MultiOpaqueRectOrder* multi_opaque_rect) { }
		public void MultiDrawNineGrid(rdpContext* context, MultiDrawNineGridOrder* multi_draw_nine_grid) { }
		public void LineTo(rdpContext* context, LineToOrder* line_to) { }
		public void Polyline(rdpContext* context, PolylineOrder* polyline) { }
		public void MemBlt(rdpContext* context, MemBltOrder* memblt) { }
		public void Mem3Blt(rdpContext* context, Mem3BltOrder* mem3blt) { }
		public void SaveBitmap(rdpContext* context, SaveBitmapOrder* save_bitmap) { }
		public void GlyphIndex(rdpContext* context, GlyphIndexOrder* glyph_index) { }
		public void FastIndex(rdpContext* context, FastIndexOrder* fast_index) { }
		public void FastGlyph(rdpContext* context, FastGlyphOrder* fast_glyph) { }
		public void PolygonSC(rdpContext* context, PolygonSCOrder* polygon_sc) { }
		public void PolygonCB(rdpContext* context, PolygonCBOrder* polygon_cb) { }
		public void EllipseSC(rdpContext* context, EllipseSCOrder* ellipse_sc) { }
		public void EllipseCB(rdpContext* context, EllipseCBOrder* ellipse_cb) { }
		
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

