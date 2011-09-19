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
using Screenary;

public partial class MainWindow: Gtk.Window
{	
	public Cairo.Surface surface;
	
	public MainWindow(): base(Gtk.WindowType.Toplevel)
	{
		Build();
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
		SurfaceCommand surfaceCommand = new SurfaceCommand();
		surfaceCommand.test();
	}

	protected void OnMainDrawingAreaExposeEvent(object o, Gtk.ExposeEventArgs args)
	{
		DrawingArea area = (DrawingArea) o;
		Cairo.Context context = Gdk.CairoHelper.Create(area.GdkWindow);
		
		Console.WriteLine("OnExposeEvent");
		
		context.SetSourceRGB(0, 178, 238);
		context.Rectangle(0, 0, 1024, 768);
		context.Fill();
	}

	protected void OnMainDrawingAreaConfigureEvent(object o, Gtk.ConfigureEventArgs args)
	{	
		DrawingArea area = (DrawingArea) o;
		Cairo.Context context = Gdk.CairoHelper.Create(area.GdkWindow);
	}
}
