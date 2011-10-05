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
using System;
using System.IO;
using Screenary;
using FreeRDP;

public partial class MainWindow : Gtk.Window
{	
	private Gdk.GC gc;
	private int width, height;
	private Gdk.Window window;
	private Gdk.Pixbuf surface;
	private Gdk.Drawable drawable;
	private SurfaceReceiver receiver;
	
	public MainWindow(): base(Gtk.WindowType.Toplevel)
	{
		Build();
		
		width = 1024;
		height = 768;
		
		window = mainDrawingArea.GdkWindow;
		drawable = (Gdk.Drawable) window;
		
		gc = new Gdk.GC(drawable);
		gc.ClipRectangle = new Gdk.Rectangle(0, 0, width, height);
		
		surface = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);		
		window.InvalidateRect(new Gdk.Rectangle(0, 0, width, height), true);
		
		receiver = new SurfaceReceiver(window, surface);
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
		BinaryReader fp;
		string filename;
		SurfaceCommand cmd;
		
		filename = "data/rfx/rfx.bin";
		fp = new BinaryReader(File.Open(filename, FileMode.Open));
		
		cmd = SurfaceCommand.Parse(fp);
		cmd.Execute(receiver);
		
		fp.Close();
	}

	protected void OnMainDrawingAreaExposeEvent(object o, Gtk.ExposeEventArgs args)
	{
		Gdk.Rectangle[] rects = args.Event.Region.GetRectangles();
		
		foreach (Gdk.Rectangle rect in rects)
		{
			drawable.DrawPixbuf(gc, surface, rect.X, rect.Y, rect.X, rect.Y,
				rect.Width, rect.Height, Gdk.RgbDither.None, 0, 0);
		}
	}

	protected void OnMainDrawingAreaConfigureEvent(object o, Gtk.ConfigureEventArgs args)
	{	

	}

	protected void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		int count = 0;
		SurfaceCommand cmd;
		MemoryStream stream;
		BinaryReader reader;
		PcapReader pcap = new PcapReader(File.OpenRead("data/rfx_sample.pcap"));
		
		foreach (PcapRecord record in pcap)
		{
			Console.WriteLine("record #{0},\ttime: {1}\tlength:{2}", count++, record.Time, record.Length);
			
			stream = new MemoryStream(record.Buffer);
			reader = new BinaryReader(stream);
			
			cmd = SurfaceCommand.Parse(reader);
			cmd.Execute(receiver);
			
			window.ProcessUpdates(false); /* force update */
		}
		
		pcap.Close();
	}
}
