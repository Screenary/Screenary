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
		
		private freerdp* handle;
		private rdpSettings* settings;
		
		private pContextNew hContextNew;
		private pContextFree hContextFree;
		
		private pPreConnect hPreConnect;
		private pPostConnect hPostConnect;
	
		private pBeginPaint hBeginPaint;
		private pEndPaint hEndPaint;
		
		private pOpaqueRect hOpaqueRect;
		
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
			
			settings = instance->settings;
		}
		
		void ContextFree(freerdp* instance, rdpContext* context)
		{
			Console.WriteLine("ContextFree");
		}
		
		bool PreConnect(freerdp* instance)
		{
			Console.WriteLine("PreConnect");
			
			hBeginPaint = new pBeginPaint(this.BeginPaint);
			hEndPaint = new pEndPaint(this.EndPaint);
			
			rdpUpdate* update = instance->update;
			
			update->BeginPaint = Marshal.GetFunctionPointerForDelegate(hBeginPaint);
			update->EndPaint = Marshal.GetFunctionPointerForDelegate(hEndPaint);
			
			Console.WriteLine("{0}", update->BeginPaint.ToString());
			
			rdpPrimaryUpdate* primary = update->primary;
			
			hOpaqueRect = new pOpaqueRect(OpaqueRect);
			primary->OpaqueRect = Marshal.GetFunctionPointerForDelegate(hOpaqueRect);
			
			return true;
		}
		
		bool PostConnect(freerdp* instance)
		{
			Console.WriteLine("PostConnect");
			
			rdpUpdate* update = instance->update;
			Console.WriteLine("{0}", update->BeginPaint.ToString());
			
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
		
		public void BeginPaint(rdpContext* context)
		{
			Console.WriteLine("BeginPaint");
		}
		
		public void EndPaint(rdpContext* context)
		{
			Console.WriteLine("EndPaint");
		}
		
		public void DstBlt(rdpContext* context, DstBltOrder* dstblt) { }
		public void PatBlt(rdpContext* context, PatBltOrder* patblt) { }
		public void ScrBlt(rdpContext* context, ScrBltOrder* scrblt) { }
		
		public void OpaqueRect(rdpContext* context, OpaqueRectOrder* opaqueRect)
		{
			Console.WriteLine("OpaqueRect");
		}
		
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
	}
}

