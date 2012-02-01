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
	internal string sessionKey;
	internal string username;
	internal string password;
	internal TransportClient transport;
	internal ArrayList participants;
	internal string creator;
	internal const int id = 1;
	internal Gtk.TextBuffer buffer = new TextBuffer(new TextTagTable());
	
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
		
		/* Set current state to STARTED */
		currentState = clientStates[STARTED_STATE];
		currentState.refresh();
		
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
		if(currentState.ToString().Equals(clientStates[RECEIVER_JOINED_STATE].ToString()))
		{
			session.SendLeaveReq();
		}
		else if(currentState.ToString().Equals(clientStates[SENDER_CREATED_STATE].ToString()))
		{			
			session.SendTermReq(sessionKey.ToCharArray());
		}
		
		if(this.transport != null)		
			this.transport.Disconnect();				
			
		Application.Quit();		

		a.RetVal = true;
	}
	
	protected void OnQuitActionActivated(object sender, System.EventArgs e)
	{				
		if(currentState.ToString().Equals(clientStates[RECEIVER_JOINED_STATE].ToString()))
		{
			session.SendLeaveReq();
		}
		else if(currentState.ToString().Equals(clientStates[SENDER_CREATED_STATE].ToString()))
		{			
			session.SendTermReq(sessionKey.ToCharArray());
		}
		
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
		OnSessionOperationFail("This method has not yet been implemented.");
	}
	
	public void OnUserConnect(string address, int port)
	{
		ChannelDispatcher dispatcher = new ChannelDispatcher();
		this.transport = new TransportClient(dispatcher);
		
		session = new Session(this.transport, this);
		dispatcher.RegisterChannel(session);
		
		SurfaceClient surface = new SurfaceClient(this, this.transport);
		dispatcher.RegisterChannel(surface);
		
		try
		{		
			this.transport.Connect(address, port);
			Console.WriteLine("connected to screenary server at {0}:{1}", address, port);
			notificationBar.Push (id, "Welcome! You are connected to Screenary server at " + address + " : " + port);
		}
		catch(Exception e)
		{
			notificationBar.Push (id, "Could not connect to Screenary server at " + address + " : " + port);
			Console.WriteLine("could not connect: "+e.ToString());
		}		
	
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
		this.creator = username;
		session.SendCreateReq(username, password);
	}
	
	public void OnUserJoinSession(string sessionKey, string username, string password)
	{
		this.username = username;
		this.password = password;
		session.SendJoinReq(sessionKey.ToCharArray());
	}

	protected void OnFreeRDPActionActivated(object sender, System.EventArgs e)
	{
		rdpSource.Connect(config.RdpServerHostname, config.RdpServerPort,
			config.RdpServerUsername, config.RdpServerDomain, config.RdpServerPassword);
	}

	protected void OnConnectActionActivated(object sender, System.EventArgs e)
	{
		OnUserConnect(config.BroadcasterHostname, config.BroadcasterPort);
	}

	public void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected)
	{
		session.SendAuthReq(username,password);
		
		Console.WriteLine("MainWindow.OnSessionJoinSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}, Password Protected:{1}", sessionKeyString, isPasswordProtected);
		
		this.sessionKey = sessionKeyString;
		
		currentState = clientStates[RECEIVER_JOINED_STATE];
		currentState.refresh();		
		
		notificationBar.Pop (id);
		notificationBar.Push (id,"You have successfully joined the session! SessionKey: " + sessionKeyString);
	}
	
	public void DisplayParticipants()
	{
			Console.WriteLine("MainWindow.DisplayParticipants()");
			
			buffer = txtParticipants.Buffer;
			buffer.Clear();
				
			if(participants != null) 
			{
				foreach(string username in participants)
				{
					buffer.InsertAtCursor(username + "\r\n");
				}
			}
	}
	
	public void DisplayCreator()
	{
			Console.WriteLine("MainWindow.DisplayCreator()");
			
			buffer = txtParticipants.Buffer;
			buffer.Clear();
			
			if(creator != null)
			{
				buffer.InsertAtCursor(creator + "\r\n");
			}
	}
	
	public void OnSessionLeaveSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionLeaveSuccess");
		
		currentState = clientStates[STARTED_STATE];
		currentState.refresh();
		
		notificationBar.Pop (id);
		notificationBar.Push (id, "You have succesfully left the session.");
	}

	public void OnSessionAuthenticationSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionAuthenticationSuccess");
		
		currentState = clientStates[RECEIVER_AUTHENTICATED_STATE];
		currentState.refresh();
	}

	public void OnSessionCreationSuccess(char[] sessionKey)
	{
		Console.WriteLine("MainWindow.OnSessionCreationSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		this.sessionKey = sessionKeyString;
		
		currentState = clientStates[SENDER_CREATED_STATE];
		Console.WriteLine("MainWindow.Switch States");

		
		currentState.refresh();
		Console.WriteLine("MainWindow.Refresh");

	
		notificationBar.Pop(id);
		Console.WriteLine("mainwindow.pop");

		notificationBar.Push (id, "You have succesfully created a session. The session key is: " + sessionKeyString);
		Console.WriteLine("MainWindow.Push");
	}

	public void OnSessionTerminationSuccess(char[] sessionKey)
	{
		Console.WriteLine("MainWindow.OnSessionTerminationSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		currentState = clientStates[STARTED_STATE];
		currentState.refresh();
		
		notificationBar.Pop(id);
		notificationBar.Push (id, "You have succesfully terminated the session.");
	}
	
	public void OnSessionOperationFail(String errorMessage)
	{
		Console.WriteLine("MainWindow.OnSessionOperationFail = " + errorMessage);
		ExceptionDialog exception = new ExceptionDialog("Operation Fail", errorMessage);
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
		
		DisplayParticipants();
	}
	
	public void OnSessionNotificationUpdate(string type, string username)
	{
		Console.WriteLine("MainWindow.OnSessionNotificationUpdate");	
		
		notificationBar.Pop(id);
		notificationBar.Push(id, username + " has " + type + " the session.");
	}

	protected void OnEndSessionActionActivated (object sender, System.EventArgs e)
	{
		session.SendTermReq(sessionKey.ToCharArray());
	}

	protected void OnLeaveSessionActionActivated (object sender, System.EventArgs e)
	{
		session.SendLeaveReq();
	}
	
	/**
	 * State interface, to be realized by concrete states (State-Pattern)
	 * Functionality is isolated that may differ in behavior based on the 
	 * State of the client
	 */ 
	public abstract class IClientState
	{
		protected MainWindow mainWindow;
		
		/**
		 * Class constructor, set main window
		 */ 
		public IClientState(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
		}
		
		/**
		 * Refresh UI layout
		 */ 
		public abstract void refresh();
	}
	
	/**
	 * Class contains functionality specific to Started State
	 */ 
	public class StartedState : IClientState
	{
		public StartedState(MainWindow mainWindow) : base(mainWindow)
		{ }
		/**
		 * Only show create or join triggers
		 */ 		
		public override void refresh()
		{
			mainWindow.vbox3.Visible = false;
			mainWindow.JoinSessionAction.Visible = true;
			mainWindow.CreateSessionAction.Visible = true;
			mainWindow.recordAction.Visible = false;
			mainWindow.LeaveSessionAction.Visible = false;
			mainWindow.EndSessionAction.Visible = false;
		}
	}	
	
	/**
	 * Class contains functionality specific to Sender-Created State
	 */ 	
	public class SenderCreatedState : IClientState
	{
		public SenderCreatedState(MainWindow mainWindow) : base(mainWindow)
		{ }
		
		/**
		 * Only show participants, recording, and end session triggers
		 */ 		
		public override void refresh()
		{
			mainWindow.vbox3.Visible = true;
			mainWindow.JoinSessionAction.Visible = false;
			mainWindow.CreateSessionAction.Visible = false;
			mainWindow.recordAction.Visible = true;
			mainWindow.LeaveSessionAction.Visible = false;
			mainWindow.EndSessionAction.Visible = true;
			
			mainWindow.DisplayCreator();
		}
	}	
	
	/**
	 * Class contains functionality specific to Receiver-Joined State
	 */ 	
	public class ReceiverJoinedState : IClientState
	{
		public ReceiverJoinedState(MainWindow mainWindow) : base(mainWindow)
		{ }
		
		/**
		 * Only show participants, recording, and leave session triggers
		 */ 		
		public override void refresh()
		{
			mainWindow.vbox3.Visible = true;
			mainWindow.JoinSessionAction.Visible = false;
			mainWindow.CreateSessionAction.Visible = false;
			mainWindow.recordAction.Visible = true;
			mainWindow.LeaveSessionAction.Visible = true;
			mainWindow.EndSessionAction.Visible = false;
		}
	}
	
	/**
	 * Class contains functionality specific to Receiver-Authenticated State
	 */ 
	public class ReceiverAuthenticatedState : IClientState
	{
		public ReceiverAuthenticatedState(MainWindow mainWindow) : base(mainWindow)
		{ }
		
		/**
		 * Only show participants, recording, and leave session triggers
		 */ 
		public override void refresh()
		{
			mainWindow.vbox3.Visible = true;
			mainWindow.JoinSessionAction.Visible = false;
			mainWindow.CreateSessionAction.Visible = false;
			mainWindow.recordAction.Visible = true;
			mainWindow.LeaveSessionAction.Visible = true;
			mainWindow.EndSessionAction.Visible = false;
		}
	}
}
