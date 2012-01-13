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
using System.Threading;
using Screenary.Client;
using Screenary;
using FreeRDP;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public partial class MainWindow : Gtk.Window, IUserAction, ISurfaceClient, ISessionResponseListener
{	
	private Gdk.GC gc;
	private Config config;
	private Thread thread;
	private Session session;
	private int width, height;
	private Gdk.Window window;
	private Gdk.Pixbuf surface;
	private Gdk.Drawable drawable;
	private SurfaceReceiver receiver;
	private int mode;
	private string sessionKey;
	
	public MainWindow(int m): base(Gtk.WindowType.Toplevel)
	{
		Build();
		
		mode = m;
		width = 1024;
		height = 768;
		
		config = Config.Load();
		
		window = mainDrawingArea.GdkWindow;
		drawable = (Gdk.Drawable) window;
		
		gc = new Gdk.GC(drawable);
		gc.ClipRectangle = new Gdk.Rectangle(0, 0, width, height);
		
		surface = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);		
		window.InvalidateRect(new Gdk.Rectangle(0, 0, width, height), true);
		
		receiver = new SurfaceReceiver(window, surface);
		
		/* Sender Mode */
		if (mode == 0)
		{
			JoinSessionAction.Visible = false;
			CreateSessionAction.Visible = true;
			recordAction.Visible = true;
			LeaveSessionAction1.Visible = false;
			EndSessionAction1.Visible = true;
		}
		
		/* Receiver Mode */
		else if (mode == 1)
		{
			CreateSessionAction.Visible = false;
			recordAction.Visible = false;
			JoinSessionAction.Visible = true;
			EndSessionAction1.Visible = false;
			LeaveSessionAction1.Visible = true;
		}
		
		if (config.BroadcasterAutoconnect)
			OnUserConnect(config.BroadcasterHostname, config.BroadcasterPort);
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
		AboutDialog about = new AboutDialog();

		/* Change the Dialog's properties to the appropriate values. */
		about.ProgramName = "Screenary";
		about.Version = "1.0.0";
		about.Comments = "A screencasting application for Ubuntu";
    	about.Authors = new string [] {"Marc Andre", "Hai-Long", "Gina", "Terri-Anne", "Marwan"};
		about.Website = "http://www.screenary.com/";
		
		/* Show the dialog and pass it control */
		about.Run();
		
		/* Destroy the dialog */
		about.Destroy();
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

	protected void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		/* Create and display a fileChooserDialog */
		FileChooserDialog chooser = new FileChooserDialog(
		   "Please select a video to view ...",
		   this, FileChooserAction.Open,
		   "Cancel", ResponseType.Cancel,
		   "Open", ResponseType.Accept);
		
		/* Open */
		if (chooser.Run() == (int) ResponseType.Accept)
		{
			/* Set the MainWindow Title to the filename. */
			this.Title = "Screenary now showing: " + chooser.Filename.ToString();
				
			/* Set the filename */
			String filename = chooser.Filename.ToString();
			
			/* Destroy the fileChooserDialog, otherwise it stays open */
			chooser.Destroy();
			
			/* PcapReader */
			int count = 0;
			SurfaceCommand cmd;
			MemoryStream stream;
			BinaryReader reader;
			
			PcapReader pcap = new PcapReader(File.OpenRead(filename));
			TimeSpan previousTime = new TimeSpan(0, 0, 0, 0);
			
			foreach (PcapRecord record in pcap)
			{
				Console.WriteLine("record #{0},\ttime: {1}\tlength:{2}", count++, record.Time, record.Length);						
				
				Thread.Sleep(record.Time.Subtract(previousTime));						
				previousTime = record.Time;
				
				stream = new MemoryStream(record.Buffer);
				reader = new BinaryReader(stream);
				
				cmd = SurfaceCommand.Parse(reader);
				cmd.Execute(receiver);
				
				window.ProcessUpdates(false); /* Force update */			
			}
			
			pcap.Close();  
		}
		else
		{
			/* Cancel */
			chooser.Destroy();
		}
	} 
	
	/* Resets the Drawing Area to blank */
	protected void OnCloseActionActivated (object sender, System.EventArgs e)
	{
		this.Title = "Screenary";
		window = mainDrawingArea.GdkWindow;
		drawable = (Gdk.Drawable) window;
		
		gc = new Gdk.GC(drawable);
		gc.ClipRectangle = new Gdk.Rectangle(0, 0, width, height);
		
		surface = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);		
		window.InvalidateRect(new Gdk.Rectangle(0, 0, width, height), true);
		
		receiver = new SurfaceReceiver(window, surface);
	}
		
	public void OnSurfaceCommand(BinaryReader s)
	{		
		Gtk.Application.Invoke(delegate {
			
			SurfaceCommand cmd = SurfaceCommand.Parse(s);
			
			if (cmd != null)
			{
				cmd.Execute(receiver);
				window.ProcessUpdates(false);
			}
		});
	}
	
	protected void OnRecordActionActivated(object sender, System.EventArgs e)
	{
		
	}
	
	public void OnUserConnect(string address, int port)
	{
		ChannelDispatcher dispatcher = new ChannelDispatcher();
		TransportClient transport = new TransportClient(dispatcher);
		
		session = new Session(transport, this);
		dispatcher.RegisterChannel(session);
		
		SurfaceClient surface = new SurfaceClient(this, transport);
		dispatcher.RegisterChannel(surface);

		transport.Connect(address, port);
		
		Console.WriteLine("connected to screenary server at {0}:{1}", address, port);

		//TA's test code
		//session.SendCreateReq("username","password");
		//session.SendTermReq("ABCDEF123456".ToCharArray());
		//session.SendJoinReq("ABCDEF123456".ToCharArray());
		//session.SendAuthReq("username","password");
		//session.SendLeaveReq();
	}
	
	protected void OnCreateSessionActionActivated(object sender, System.EventArgs e)
	{
		CreateSessionDialog connect = new CreateSessionDialog(this);
		
	}
	
	public void OnUserCreateSession(string username, string password)
	{
		session.SendCreateReq(username, password);
	}
	
	public void OnUserJoinSession(string sk)
	{
		session.SendJoinReq(sk.ToCharArray());
	}

	protected void OnJoinSessionActionActivated(object sender, System.EventArgs e)
	{
		JoinDialog join = new JoinDialog(this);
	}
	
	protected void OnExposeEvent(object o, Gtk.ExposeEventArgs args)
	{
	}

	protected void OnFreeRDPActionActivated(object sender, System.EventArgs e)
	{
		RDP rdp = new RDP();
		
		rdp.Connect();
		
		thread = new Thread(() => ThreadProc(rdp));
		thread.Start();
	}
	
	static void ThreadProc(RDP rdp)
	{
		while (true)
		{
			rdp.CheckFileDescriptor();
			Thread.Sleep(10);
		}
	}

	protected void OnSenderActionActivated(object sender, System.EventArgs e)
	{
		/* Switch to Sender mode */
		JoinSessionAction.Visible = false;
		CreateSessionAction.Visible = true;
		recordAction.Visible = true;
		LeaveSessionAction1.Visible = false;
		EndSessionAction1.Visible = true;
		
	}

	protected void OnReceiverActionActivated(object sender, System.EventArgs e)
	{
		/* Switch to Receiver mode */
		CreateSessionAction.Visible = false;
		recordAction.Visible = false;
		JoinSessionAction.Visible = true;
		EndSessionAction1.Visible = false;
		LeaveSessionAction1.Visible = true;
	}

	protected void OnConnectAction1Activated(object sender, System.EventArgs e)
	{
		OnUserConnect(config.BroadcasterHostname, config.BroadcasterPort);
	}
	
	protected void OnConnectActionActivated(object sender, System.EventArgs e)
	{

	}

	public void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected)
	{
		Console.WriteLine("MainWindow.OnSessionJoinSuccess");
		string sessionKeyString = "";
		for(int i = 0; i < sessionKey.Length; i++) {
			sessionKeyString += sessionKey[i];
		}
		Console.WriteLine("SessionKey:{0}, Password Protected:{1}", sessionKeyString, isPasswordProtected);
	}
	
	public void OnSessionLeaveSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionLeaveSuccess");
	}

	public void OnSessionAuthenticationSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionAuthenticationSuccess");
	}

	public void OnSessionCreationSuccess(char[] sk)
	{
		Console.WriteLine("MainWindow.OnSessionCreationSuccess");
		string sessionKeyString = "";
		for(int i = 0; i < sk.Length; i++) {
			sessionKeyString += sk[i];
		}
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		sessionKey = sessionKeyString;
	}

	public void OnSessionTerminationSuccess(char[] sk)
	{
		Console.WriteLine("MainWindow.OnSessionTerminationSuccess");
		string sessionKeyString = "";
		for(int i = 0; i < sk.Length; i++) {
			sessionKeyString += sk[i];
		}
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
	}
	
	public void OnSessionOperationFail(String errorMessage)
	{
		Console.WriteLine("MainWindow.OnSessionOperationFail");
		Console.WriteLine(errorMessage);
	}

	protected void OnEndSessionActionActivated(object sender, System.EventArgs e)
	{
	
	}

	protected void OnLeaveSessionActionActivated(object sender, System.EventArgs e)
	{
	}

	protected void OnEndSessionAction1Activated (object sender, System.EventArgs e)
	{
		session.SendTermReq(sessionKey.ToCharArray());
	}

	protected void OnLeaveSessionAction1Activated (object sender, System.EventArgs e)
	{
		session.SendLeaveReq();
	}

}
