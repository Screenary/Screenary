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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

public partial class MainWindow : Gtk.Window, IUserAction, ISurfaceClient, ISource, ISessionResponseListener
{	
	internal Gdk.GC gc;
	internal Config config;
	internal SessionClient sessionClient;
	internal SurfaceClient surfaceClient;
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
	private uint messageId = 0;
	private uint contextId = 1;
	private readonly object mainLock = new object();
	
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
		clientStates = new IClientState[4]
		{
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
		
		mainDrawingArea.AddEvents(
			(int) Gdk.EventMask.ButtonPressMask |
			(int) Gdk.EventMask.ButtonReleaseMask |
			(int) Gdk.EventMask.PointerMotionMask |
			(int) Gdk.EventMask.KeyPressMask |
			(int) Gdk.EventMask.KeyReleaseMask);
		
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
	
	protected void OnMainDrawingAreaButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
	{
		/*Gdk.EventButton e = args.Event;
		
		Console.WriteLine("ButtonPressEvent button:{0} ({1},{2}) ({3},{4})",
			e.Button, e.X, e.Y, e.XRoot, e.YRoot);*/
	}
	
	protected void OnMainDrawingAreaButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
	{
		/*Gdk.EventButton e = args.Event;
		
		Console.WriteLine("ButtonReleaseEvent button:{0} ({1},{2}) ({3},{4})",
			e.Button, e.X, e.Y, e.XRoot, e.YRoot);*/
	}
	
	protected void OnMainDrawingAreaMotionNotifyEvent(object o, Gtk.MotionNotifyEventArgs args)
	{
		/*Gdk.EventMotion e = args.Event;
		
		Console.WriteLine("MotionNotifyEvent ({0},{1}) ({2},{3})",
			e.X, e.Y, e.XRoot, e.YRoot);*/
	}
	
	protected void OnMainDrawingAreaKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
	{
		/*Gdk.EventKey e = args.Event;

		Console.WriteLine("KeyPressEvent key:{0} keyValue:{1} hardwareKeyCode:{2}",
			e.Key, e.KeyValue, e.HardwareKeycode);*/
	}
	
	protected void OnMainDrawingAreaKeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs args)
	{
		/*Gdk.EventKey e = args.Event;

		Console.WriteLine("KeyReleaseEvent key:{0} keyValue:{1} hardwareKeyCode:{2}",
			e.Key, e.KeyValue, e.HardwareKeycode);*/
	}

	protected void OnMainDrawingAreaFocusInEvent(object o, Gtk.FocusInEventArgs args)
	{

	}

	protected void OnMainDrawingAreaFocusOutEvent(object o, Gtk.FocusOutEventArgs args)
	{
		
	}
	
	/**
	 * Configure the drawing area when the application launches
	 **/
	protected void OnMainDrawingAreaExposeEvent(object o, Gtk.ExposeEventArgs args)
	{
		Gdk.Rectangle[] rects = args.Event.Region.GetRectangles();
		
		foreach (Gdk.Rectangle rect in rects)
		{
			drawable.DrawPixbuf(gc, surface, rect.X, rect.Y, rect.X, rect.Y,
				rect.Width, rect.Height, Gdk.RgbDither.None, 0, 0);
		}
	}
	
	/**
	 * When application is closed notify the sender/receiver and disconnect from server
	 **/
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{				
		if (currentState.ToString().Equals(clientStates[RECEIVER_JOINED_STATE].ToString()))
		{
			sessionClient.SendLeaveReq(username);
		}
		else if (currentState.ToString().Equals(clientStates[SENDER_CREATED_STATE].ToString()))
		{			
			sessionClient.SendTermReq(sessionKey.ToCharArray());
		}
		
		if (this.transport != null)		
			this.transport.Disconnect();				
			
		Application.Quit();		

		a.RetVal = true;
	}
	
	/**
	 * When application is exited notify the sender/receiver and disconnect from server 
	 **/
	protected void OnQuitActionActivated(object sender, System.EventArgs e)
	{				
		if (currentState.ToString().Equals(clientStates[RECEIVER_JOINED_STATE].ToString()))
		{
			sessionClient.SendLeaveReq(username);
		}
		else if (currentState.ToString().Equals(clientStates[SENDER_CREATED_STATE].ToString()))
		{			
			sessionClient.SendTermReq(sessionKey.ToCharArray());
		}
		
		if (this.transport != null)				
			this.transport.Disconnect();							
		
		Application.Quit();		
	}
	
	/**
	 * About Dialog opens to display information about the application 
	 **/
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
	
	/**
	 * Menu item currently being used as a fill in. Opens a single frame? 
	 **/
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
	
	/**
	 * Open a file for playback. A file chooser dialog opens and allows the user to select the file they want to open. 
	 **/
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
	
	/**
	 * Resets the Drawing Area to blank 
	 **/
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
		byte[] pduBuffer = cmd.Write();
		
		surfaceClient.SendSurfaceCommand(pduBuffer, sessionClient.GetSessionId());
		
		Gtk.Application.Invoke(delegate {
			
			if (cmd != null)
			{
				cmd.Execute(receiver);
				window.ProcessUpdates(false);
			}
		});
	}
	
	/**
	 * Set the application into Record State [NOT IMPLEMENTED YET]
	 **/
	protected void OnRecordActionActivated(object sender, System.EventArgs e)
	{
		rdpSource.Connect(config.RdpServerHostname, config.RdpServerPort,
			config.RdpServerUsername, config.RdpServerDomain, config.RdpServerPassword);
	}
	
	/**
	 * Is invoked when application launches. Connects the Client to the Server
	 **/
	public void OnUserConnect(string address, int port)
	{
		ChannelDispatcher dispatcher = new ChannelDispatcher();
		this.transport = new TransportClient(dispatcher);
		
		sessionClient = new SessionClient(this.transport, this);
		dispatcher.RegisterChannel(sessionClient);
		
		surfaceClient = new SurfaceClient(this, this.transport);
		dispatcher.RegisterChannel(surfaceClient);
		
		try
		{
			this.transport.Connect(address, port);
			Console.WriteLine("connected to screenary server at {0}:{1}", address, port);
			DisplayStatusText("Welcome! You are connected to Screenary server at " + address + " : " + port);
		}
		catch(Exception e)
		{
			DisplayStatusText("Could not connect to Screenary server at " + address + " : " + port);
			Console.WriteLine("could not connect: " + e.ToString());
		}
	}
	
	/**
	 * When Create Session is activated launch the Create Session Dialog
	 **/
	protected void OnCreateSessionActionActivated(object sender, System.EventArgs e)
	{
		CreateSessionDialog connect = new CreateSessionDialog(this);
	}
	
	/**
	 * When Join Session is activated launch the Join Dialog
	 **/
	protected void OnJoinSessionActionActivated(object sender, System.EventArgs e)
	{
		JoinDialog join = new JoinDialog(this);
	}
	
	/**
	 * Called from the Create Session Dialog, sends the create request
	 **/
	public void OnUserCreateSession(string username, string password)
	{
		if (this.transport.isConnected())
		{
			this.creator = username;
			this.username = username;
			sessionClient.SendCreateReq(username, password);
		}
		else
		{
			DisplayStatusText("Please connect to the server first");
		}
	}
	
	/**
	 * Called from the Join Dialog, sends the join request
	 **/
	public void OnUserJoinSession(string sessionKey, string username, string password)
	{
		if (this.transport.isConnected())
		{
			this.username = username;
			this.password = password;
			sessionClient.SendJoinReq(sessionKey.ToCharArray());
		}
		else
		{
			DisplayStatusText("Please connect to the server first");
		}
	}
	
	/**
	 * Will be used once FreeRDP is functioning [NOT IMPLEMENTED YET] 
	 **/
	protected void OnFreeRDPActionActivated(object sender, System.EventArgs e)
	{
		rdpSource.Connect(config.RdpServerHostname, config.RdpServerPort,
			config.RdpServerUsername, config.RdpServerDomain, config.RdpServerPassword);
	}
	
	/**
	 * When Connect is activated, try OnUserConnect
	 **/
	protected void OnConnectActionActivated(object sender, System.EventArgs e)
	{
		OnUserConnect(config.BroadcasterHostname, config.BroadcasterPort);
	}
	
	/**
	 * When a session is joined succesfully, change to the receiver state, refresh, and update notification bar 
	 **/
	public void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected)
	{
		sessionClient.SendAuthReq(username,password);
		
		Console.WriteLine("MainWindow.OnSessionJoinSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}, Password Protected:{1}", sessionKeyString, isPasswordProtected);
		
		this.sessionKey = sessionKeyString;
		
		currentState = clientStates[RECEIVER_JOINED_STATE];
		currentState.refresh();
		
		DisplayStatusText("You have successfully joined the session! SessionKey: " + sessionKeyString);
	}
	
	/**
	 * Display/update the participants list
	 **/
	public void DisplayParticipants()
	{
		Console.WriteLine("MainWindow.DisplayParticipants()");
		
		Gtk.Application.Invoke(delegate {
		
			txtParticipants.Buffer.Clear();
	
			if (participants != null)
			{
				foreach (string username in participants)
				{
					txtParticipants.Buffer.InsertAtCursor(username + "\r\n");
				}
			}
			
		});
	}
	
	/** 
	 * Display/update the participants list for the sender 
	 **/
	public void DisplayCreator()
	{
		Console.WriteLine("MainWindow.DisplayCreator()");
		
		Gtk.Application.Invoke(delegate {

			txtParticipants.Buffer.Clear();
			
			if (creator != null)
			{
				txtParticipants.Buffer.InsertAtCursor(creator + "\r\n");
			}
		});
	}
	
	public void DisplayStatusText(string text)
	{
		Gtk.Application.Invoke(delegate {
			
			if (messageId != 0)
				notificationBar.Remove(contextId, messageId);
				
			messageId = notificationBar.Push(contextId, text);
		});
	}
	
	/**
	 * When a session is left succesfully, change to the starter state, refresh, and update notification bar
	 **/
	public void OnSessionLeaveSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionLeaveSuccess");
		
		currentState = clientStates[STARTED_STATE];
		currentState.refresh();
		
		DisplayStatusText("You have succesfully left the session.");
	}
	
	/**
	 * When a users has succesfully authenticated into a session, change to the authenticated state, refresh, and update notification bar 
	 **/
	public void OnSessionAuthenticationSuccess()
	{
		Console.WriteLine("MainWindow.OnSessionAuthenticationSuccess");
		
		currentState = clientStates[RECEIVER_AUTHENTICATED_STATE];
		currentState.refresh();
	}
	
	/**
	 * When a session is created succesfully, change to the sender state, refresh, and update notification bar
	 **/
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

		DisplayStatusText("You have succesfully created a session. The session key is: " + sessionKeyString);
	}
	
	/**
	 * When a session is terminated succesfully, change to the starter state, refresh, and update notification bar
	 **/
	public void OnSessionTerminationSuccess(char[] sessionKey)
	{
		Console.WriteLine("MainWindow.OnSessionTerminationSuccess");
		string sessionKeyString = new string(sessionKey);
		Console.WriteLine("SessionKey:{0}", sessionKeyString);
		
		currentState = clientStates[STARTED_STATE];
		currentState.refresh();
		
		DisplayStatusText("You have succesfully terminated the session.");
	}
	
	/**
	 * Default Exception [FOR NOW]
	 **/
	public void OnSessionOperationFail(String errorMessage)
	{
		Console.WriteLine("MainWindow.OnSessionOperationFail = " + errorMessage);
		ExceptionDialog exception = new ExceptionDialog("Operation Fail", errorMessage);
	}
	
	/**
	 * Updates the participants lists
	 **/
	public void OnSessionParticipantListUpdate(ArrayList participants)
	{
		Console.WriteLine("MainWindow.OnSessionPartipantsListUpdate");
		this.participants = participants;
			
		DisplayParticipants();
	}
	
	/**
	 * Notifies when a user has joined/left a session
	 **/
	public void OnSessionNotificationUpdate(string type, string username)
	{
		Console.WriteLine("MainWindow.OnSessionNotificationUpdate");
		
		if (this.username.Equals(username) && type.Equals("joined"))
		{
			DisplayStatusText("You have successfully joined the session.");
		}
		else
		{
			DisplayStatusText(username + " has " + type + " the session.");
		}
	}
	
	/**
	 * When end session is activated it sends the termination request
	 **/
	protected void OnEndSessionActionActivated(object sender, System.EventArgs e)
	{
		sessionClient.SendTermReq(sessionKey.ToCharArray());
	}
	
	/** 
	 * When leave session is activated it sends the leave request 
	 **/
	protected void OnLeaveSessionActionActivated(object sender, System.EventArgs e)
	{
		sessionClient.SendLeaveReq(username);
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
		 **/ 		
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
		 **/ 		
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
		 **/ 
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
