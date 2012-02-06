using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Screenary;

namespace Screenary.Server
{
	public class Client : ISessionRequestListener, ISurfaceServer
	{
		private SessionServer sessionServer;
		private SurfaceServer surfaceServer;
		private ChannelDispatcher dispatcher;
		private IClientListener clientListener;
		
		/**
		 * Class constructor
		 */ 
		public Client(TransportClient transport, IClientListener clientListener)
		{			
			this.clientListener = clientListener;
			
			dispatcher = new ChannelDispatcher();
			
			transport.SetChannelDispatcher(dispatcher);

			surfaceServer = new SurfaceServer(transport, this);
			dispatcher.RegisterChannel(surfaceServer);
			
			sessionServer = new SessionServer(transport, this);
			dispatcher.RegisterChannel(sessionServer);
			
			transport.StartThread();
			
			dispatcher.OnConnect();
		}
		
		public void OnSessionJoinRequested(char[] sessionKey)
		{
			Console.WriteLine("Client.OnSessionJoinRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionKey:{0}", sessionKeyString);
			
			UInt32 sessionId = UInt32.MaxValue;
			UInt32 sessionStatus = UInt32.MaxValue;
			byte sessionFlags = 0x00;
			
			clientListener.OnSessionJoinRequested(this, sessionKey, ref sessionId, ref sessionStatus, ref sessionFlags);
			sessionServer.SendJoinRsp(sessionId, sessionKey, sessionStatus, sessionFlags);
		}
		
		public void OnSessionLeaveRequested(UInt32 sessionId, string username)
		{
			Console.WriteLine("Client.OnSessionLeaveRequested");
			Console.WriteLine("sessionId: {0}", sessionId);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnSessionLeaveRequested(this, sessionId, sessionServer.sessionKey, ref sessionStatus, username);
			sessionServer.SendLeaveRsp(sessionId, sessionStatus);		
		}

		public void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password)
		{
			Console.WriteLine("Client.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnSessionAuthenticationRequested(this, sessionId, sessionServer.sessionKey, username, password, ref sessionStatus);
			sessionServer.SendAuthRsp(sessionId, sessionStatus);
		}
		
		public void OnSessionCreateRequested(string username, string password)
		{
			Console.WriteLine("Client.OnSessionCreateRequested");
			Console.WriteLine("username:{0} password:{1}", username, password);
			
			UInt32 sessionId = UInt32.MaxValue;
			char[] sessionKey = "000000000000".ToCharArray();
			
			clientListener.OnSessionCreateRequested(this, username, password, ref sessionId, ref sessionKey);
			sessionServer.SendCreateRsp(sessionId, sessionKey);
		}
		
		public void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Client.OnSessionTerminationRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);

			clientListener.OnSessionTerminationRequested(this, sessionId, sessionKey, ref sessionStatus);
			sessionServer.SendTermRsp(sessionId, sessionKey, sessionStatus);
		}	
		
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("Client.OnSessionOperationFail");
			Console.WriteLine("errorMessage: {0}", errorMessage);
		}	
		
		public void OnSessionParticipantListUpdated(ArrayList participants)
		{
			Console.WriteLine("Client.OnSessionParticipantsListUpdated");
			sessionServer.SendParticipantsListRsp(participants);
		}	
		
		public void OnSessionNotificationUpdate(string type, string username)
		{
			Console.WriteLine("Client.OnSessionNotificationUpdate");
			sessionServer.SendNotificationRsp(type, username);
		}
		
		public void OnSurfaceCommand(byte[] buffer)
		{
			Console.WriteLine("Client.OnSurfaceCommand");
		}
	}
}