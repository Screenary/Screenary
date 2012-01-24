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
using System.Collections;

public partial class MainWindow : Gtk.Window, IUserAction, ISurfaceClient, ISource, ISessionResponseListener
{	
	internal Gdk.GC gc;
	internal Config config;
	internal Session session;
	internal int width, height;
	internal Gdk.Window window;
	internal Gdk.Pixbuf surface;
	internal Gdk.Drawable drawable;
	internal SurfaceReceiver receiver;
	internal int mode;
	internal string sessionKey;
	internal TransportClient transport;
	internal ArrayList participants;
	internal const int id = 1;
	
	internal RdpSource rdpSource;
	internal PcapSource pcapSource;
	
	internal IClientState currentState;
	internal IClientState[] clientStates;
	internal static int STARTED_STATE = 0;
	internal static int SENDER_CREATED_STATE = 1;
	internal static int RECEIVER_JOINED_STATE = 2;
	internal static int RECEIVER_AUTHENTICATED_STATE = 3;
	
	public MainWindow(int m): base(Gtk.WindowType.Toplevel)
	{
		Build();
		
		/* Instantiate client states */
		clientStates = new IClientState[4] {
			new StartedState(this),
			new SenderCreatedState(this),
			new ReceiverJoinedState(this),
			new ReceiverAuthenticatedState(this)
		};
		currentState = clientStates[STARTED_STATE];
		
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
		
		OutOfSessionWindow();
		
		this.transport = null;		
		
		rdpSource = new RdpSource(this);
		pcapSource = new PcapSource(this);
		
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
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		if(this.transport != null)
			this.transport.Disconnect();
		
		Application.Quit();
		a.RetVal = true;
	}
	
//Window Setups:
	protected void InSessionWindow()
	{
		this.vbox3.Visible = true;
	}
	
	protected void OutOfSessionWindow()
	{
		this.vbox3.Visible = false;
	}
	

	protected void OnQuitActionActivated(object sender, System.EventArgs e)
	{		
		if(this.transport != null)
			this.transport.Disconnect();
		
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
			
			/* Start playing asynchronously */
			pcapSource.Play(filename);
			
			/* Destroy the fileChooserDialog, otherwise it stays open */
			chooser.Destroy();
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
	
	public void OnSurfaceCommand(SurfaceCommand cmd)
	{		
		Gtk.Application.Invoke(delegate {
			
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
		this.transport = new TransportClient(dispatcher);
		
		session = new Session(this.transport, this);
		dispatcher.RegisterChannel(session);
		
		SurfaceClient surface = new SurfaceClient(this, this.transport);
		dispatcher.RegisterChannel(surface);

		this.transport.Connect(address, port);
		
		Console.WriteLine("connected to screenary server at {0}:{1}", address, port);
		notificationBar.Push (id, "Welcome! You are connected to Screenary server at " + address + " : " + port);
	}
	
	protected void OnCreateSessionActionActivated(object sender, System.EventArgs e)
	{
		CreateSessionDialog connect = new CreateSessionDialog(this);
	}
	
	protected void OnJoinSessionActionActivated(object sender, System.EventArgs e)
	{
		JoinDialog join = new JoinDialog(this);
	}
	
	public void OnUserCreateSession(string username, string password)
	{
		session.SendCreateReq(username, password);
	}
	
	public void OnUserJoinSession(string sessionKey)
	{
		session.SendJoinReq(sessionKey.ToCharArray());
	}
	
	public void OnUserAuthenticateSession(string sessionKey, string username, string password)
	{		
		session.SendAuthReq(username, password);
	}

	protected void OnFreeRDPActionActivated(object sender, System.EventArgs e)
	{
		rdpSource.Connect(config.RdpServerHostname, config.RdpServerPort,
			config.RdpServerUsername, config.RdpServerDomain, config.RdpServerPassword);
	}

	protected void OnSenderActionActivated(object sender, System.EventArgs e)
	{
		/* Switch to Sender mode */
		JoinSessionAction.Visible = false;
		CreateSessionAction.Visible = true;
		recordAction.Visible = true;
		LeaveSessionAction.Visible = false;
		EndSessionAction.Visible = true;
	}

	protected void OnReceiverActionActivated(object sender, System.EventArgs e)
	{
		/* Switch to Receiver mode */
		CreateSessionAction.Visible = false;
		recordAction.Visible = false;
		JoinSessionAction.Visible = true;
		EndSessionAction.Visible = false;
		LeaveSessionAction.Visible = true;
	}

	protected void OnConnectActionActivated(object sender, System.EventArgs e)
	{
		OnUserConnect(config.BroadcasterHostname, config.BroadcasterPort);
	}

	public void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected)
	{
		Console.WriteLine("MainWindow.OnSessionJoinSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}, Password Protected:{1}", sessionKeyString, isPasswordProtected);
		//AuthenticateDialog authentication = new AuthenticateDialog(this, sessionKeyString); //TODO this causes errors and I don't know why (TA)
		session.SendAuthReq("terri", "anne");//temp code until statement above works.
				
		notificationBar.Pop (id);
		notificationBar.Push (id,"You have successfully joined the session! SessionKey: " + sessionKeyString);
		
		InSessionWindow();
		
		DisplayParticipants();
	}
	
	public void DisplayParticipants()
	{
		if(participants != null) 
		{
			Gtk.TextBuffer buffer;
			buffer = txtParticipants.Buffer;
		
			foreach(string username in participants)
			{
				buffer.InsertAtCursor(username + "\r\n");
			}
		}
	}
	
	public void OnSessionLeaveSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionLeaveSuccess");
		
		OutOfSessionWindow();
		
		notificationBar.Pop (id);
		notificationBar.Push (id, "You have succesfully left the session.");
	}

	public void OnSessionAuthenticationSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionAuthenticationSuccess");
		
		InSessionWindow();
	}

	public void OnSessionCreationSuccess(char[] sessionKey)
	{
		Console.WriteLine("MainWindow.OnSessionCreationSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		this.sessionKey = sessionKeyString;
		
		InSessionWindow();
		
		DisplayParticipants();
	
		notificationBar.Pop(id);
		notificationBar.Push (id, "You have succesfully created a session. The session key is: " + sessionKeyString);
	}

	public void OnSessionTerminationSuccess(char[] sessionKey)
	{
		Console.WriteLine("MainWindow.OnSessionTerminationSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		OutOfSessionWindow();
		
		notificationBar.Pop(id);
		notificationBar.Push (id, "You have succesfully terminated the session.");
	}
	
	public void OnSessionOperationFail(String errorMessage)
	{
		Console.WriteLine("MainWindow.OnSessionOperationFail = " + errorMessage);
	}

	public void OnSessionParticipantListUpdate(ArrayList participants)
	{
		Console.WriteLine("MainWindow.OnSessionPartipantsListUpdate");		
		this.participants = participants;
		
		Console.WriteLine("participants.Count " + participants.Count);
		
		foreach(string username in participants)
		{
			Console.WriteLine("participant: " + username);
		}
	}

	protected void OnEndSessionActionActivated (object sender, System.EventArgs e)
	{
		session.SendTermReq(sessionKey.ToCharArray());
	}

	protected void OnLeaveSessionActionActivated (object sender, System.EventArgs e)
	{
		session.SendLeaveReq();
	}
	
}
