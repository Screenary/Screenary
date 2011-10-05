using System;

namespace FreeRDP
{
	public class SurfaceReceiver
	{
		public Rfx rfx;
		public Gdk.Window window;
		public Gdk.Pixbuf surface;
		
		public SurfaceReceiver(Gdk.Window window, Gdk.Pixbuf surface)
		{
			rfx = new Rfx();
			this.window = window;
			this.surface = surface;
		}
		
		public void InvalidateRect(int x, int y, int width, int height)
		{
			window.InvalidateRect(new Gdk.Rectangle(x, y, 64, 64), true);
		}
	}
}

