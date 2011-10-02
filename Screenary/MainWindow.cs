/**
 * Screenary: Real-Time Collaboration Redefined.
 * Main Window
 *
 * Copyright 2011 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Gtk;
using Gdk;
using System;
using System.IO;
using Screenary;
using FreeRDP;

public partial class MainWindow: Gtk.Window
{	
	public Cairo.Surface surface;
	private byte[] buffer = new byte[4096 * 4];
	
	public MainWindow(): base(Gtk.WindowType.Toplevel)
	{
		Build();
		
		Cairo.Context context = Gdk.CairoHelper.Create(mainDrawingArea.GdkWindow);
		
		context.SetSourceRGB(0, 0, 0);
		context.Rectangle(0, 0, 1024, 768);
		context.Fill();
		
		((IDisposable) context).Dispose();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnQuitActionActivated(object sender, System.EventArgs e)
	{
		Application.Quit();
	}

	protected void OnAboutActionActivated(object sender, System.EventArgs e)
	{
		AboutDialog dialog = new AboutDialog();
		dialog.Run();
	}

	protected void OnRemoteFXActionActivated(object sender, System.EventArgs e)
	{
		Rfx rfx;
		int x, y;
		BinaryReader fp;
		string filename;
		SurfaceCommand cmd;
		Gdk.Pixbuf surface;
		
		x = y = 0;
		filename = "data/rfx/rfx.bin";
		fp = new BinaryReader(File.Open(filename, FileMode.Open));
		
		rfx = new Rfx();
		cmd = SurfaceCommand.Parse(fp);
		Gdk.GC gc = new Gdk.GC(mainDrawingArea.GdkWindow);
		
		Gdk.Drawable drw = (Gdk.Drawable) mainDrawingArea.GdkWindow;
		
		if (cmd.cmdType == SurfaceCommand.CMDTYPE_STREAM_SURFACE_BITS)
		{
			SurfaceBitsCommand bitsCmd = (SurfaceBitsCommand) cmd;
			RfxMessage rfxMsg = rfx.ParseMessage(bitsCmd.bitmapData, bitsCmd.bitmapDataLength);
			
			while (rfxMsg.HasNextTile())
			{
				rfxMsg.GetNextTile(buffer, ref x, ref y);
				surface = new Gdk.Pixbuf(buffer, Gdk.Colorspace.Rgb, true, 8, 64, 64, 64 * 4);
				drw.DrawPixbuf(gc, surface, 0, 0, x, y, 64, 64, RgbDither.Normal, 0, 0);	
			}
		}
		
		fp.Close();
	}

	protected void OnMainDrawingAreaExposeEvent(object o, Gtk.ExposeEventArgs args)
	{

	}

	protected void OnMainDrawingAreaConfigureEvent(object o, Gtk.ConfigureEventArgs args)
	{	

	}
}
