using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FreeRDP
{	
	public unsafe class RDP: IUpdate, IPrimaryUpdate
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
		
		private freerdp* handle;
		private rdpContext* context;
		private rdpSettings* settings;
		
		private pContextNew hContextNew;
		private pContextFree hContextFree;
		
		private pPreConnect hPreConnect;
		private pPostConnect hPostConnect;
		
		private Update update;
		private PrimaryUpdate primary;
		
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
			
			update = new Update(instance->context);
			update.RegisterInterface(this);
			
			primary = new PrimaryUpdate(instance->context);
			primary.RegisterInterface(this);
			
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
			
			settings->hostname = GetNativeAnsiString("192.168.1.175");
			settings->username = GetNativeAnsiString("Administrator");
			settings->password = GetNativeAnsiString("FreeRDP123!");
			
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
		
		public void BeginPaint(rdpContext* context) { }
		public void EndPaint(rdpContext* context) { }
		public void SetBounds(rdpContext* context, rdpBounds* bounds) { }
		public void Synchronize(rdpContext* context) { }
		public void DesktopResize(rdpContext* context) { }
		public void BitmapUpdate(rdpContext* context, BitmapUpdate* bitmap) { }
		public void Palette(rdpContext* context, PaletteUpdate* palette) { }
		public void PlaySound(rdpContext* context, PlaySoundUpdate* playSound) { }
		public void SurfaceBits(rdpContext* context, SurfaceBitsCmd* surfaceBitsCmd) { }
		
		public void DstBlt(rdpContext* context, DstBltOrder* dstblt)
		{
			Console.WriteLine("DstBlt");
		}
		
		public void PatBlt(rdpContext* context, PatBltOrder* patblt)
		{
			Console.WriteLine("PatBlt");
		}
		
		public void ScrBlt(rdpContext* context, ScrBltOrder* scrblt)
		{
			Console.WriteLine("ScrBlt");
		}
		
		public void OpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect)
		{
			Console.WriteLine("OpaqueRect");
		}
		
		public void DrawNineGrid(rdpContext* context, DrawNineGridOrder* drawNineGrid)
		{
			Console.WriteLine("DrawNineGrid");
		}
		
		public void MultiDstBlt(rdpContext* context, MultiDstBltOrder* multi_dstblt)
		{
			Console.WriteLine("MultiDstBlt");
		}
		
		public void MultiPatBlt(rdpContext* context, MultiPatBltOrder* multi_patblt)
		{
			Console.WriteLine("MultiPatBlt");
		}
		
		public void MultiScrBlt(rdpContext* context, MultiScrBltOrder* multi_scrblt)
		{
			Console.WriteLine("MultiScrBlt");
		}
		
		public void MultiOpaqueRect(rdpContext* context, MultiOpaqueRectOrder* multi_opaque_rect)
		{
			Console.WriteLine("MultiOpaqueRect");
		}
		
		public void MultiDrawNineGrid(rdpContext* context, MultiDrawNineGridOrder* multi_draw_nine_grid)
		{
			Console.WriteLine("MultiDrawNineGrid");
		}
		
		public void LineTo(rdpContext* context, LineToOrder* line_to)
		{
			Console.WriteLine("LineTo");
		}
		
		public void Polyline(rdpContext* context, PolylineOrder* polyline)
		{
			Console.WriteLine("Polyline");
		}
		
		public void MemBlt(rdpContext* context, MemBltOrder* memblt)
		{
			Console.WriteLine("MemBlt");
		}
		
		public void Mem3Blt(rdpContext* context, Mem3BltOrder* mem3blt)
		{
			Console.WriteLine("Mem3Blt");
		}
		
		public void SaveBitmap(rdpContext* context, SaveBitmapOrder* save_bitmap)
		{
			Console.WriteLine("SaveBitmap");
		}
		
		public void GlyphIndex(rdpContext* context, GlyphIndexOrder* glyph_index)
		{
			Console.WriteLine("GlyphIndex");
		}
		
		public void FastIndex(rdpContext* context, FastIndexOrder* fast_index)
		{
			Console.WriteLine("FastIndex");
		}
		
		public void FastGlyph(rdpContext* context, FastGlyphOrder* fast_glyph)
		{
			Console.WriteLine("FastGlyph");
		}
		
		public void PolygonSC(rdpContext* context, PolygonSCOrder* polygon_sc)
		{
			Console.WriteLine("PolygonSC");
		}
		
		public void PolygonCB(rdpContext* context, PolygonCBOrder* polygon_cb)
		{
			Console.WriteLine("PolygonCB");
		}
		
		public void EllipseSC(rdpContext* context, EllipseSCOrder* ellipse_sc)
		{
			Console.WriteLine("EllipseSC");
		}
		
		public void EllipseCB(rdpContext* context, EllipseCBOrder* ellipse_cb)
		{
			Console.WriteLine("EllipseCB");
		}
	}
}

