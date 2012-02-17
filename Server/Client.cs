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
		private InputServer inputServer;
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
			
			inputServer = new InputServer(transport, this);
			dispatcher.RegisterChannel(inputServer);
			
			transport.StartThread();
			
			dispatcher.OnConnect();
		}
		
		public void OnSessionJoinRequested(char[] sessionKey)
		{
			UInt32 sessionId = UInt32.MaxValue;
			UInt32 sessionStatus = UInt32.MaxValue;
			byte sessionFlags = 0x00;
			
			clientListener.OnSessionJoinRequested(this, sessionKey, ref sessionId, ref sessionStatus, ref sessionFlags);
			sessionServer.SendJoinRsp(sessionId, sessionKey, sessionStatus, sessionFlags);
		}
		
		public void OnSessionLeaveRequested(UInt32 sessionId, string username)
		{
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnSessionLeaveRequested(this, sessionId, sessionServer.sessionKey, ref sessionStatus, username);
			sessionServer.SendLeaveRsp(sessionId, sessionStatus);		
		}

		public void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password)
		{
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnSessionAuthenticationRequested(this, sessionId, sessionServer.sessionKey, username, password, ref sessionStatus);
			sessionServer.SendAuthRsp(sessionId, sessionStatus);
		}
		
		public void OnSessionCreateRequested(string username, string password)
		{			
			UInt32 sessionId = UInt32.MaxValue;
			char[] sessionKey = "000000000000".ToCharArray();
			
			clientListener.OnSessionCreateRequested(this, username, password, ref sessionId, ref sessionKey);
			sessionServer.SendCreateRsp(sessionId, sessionKey);
		}
		
		public void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			clientListener.OnSessionTerminationRequested(this, sessionId, sessionKey, ref sessionStatus);
			sessionServer.SendTermRsp(sessionId, sessionKey, sessionStatus);
		}	
		
		/**
		 * Receiver sends request to Broadcaster
		 */
		public void OnSessionScreenControlRequested(char[] sessionKey, string username)
		{
			clientListener.OnSessionScreenControlRequested(this, sessionKey, username);
		}
		
		/**
		 * Broadcaster informs Sender of the request
		 */
		public void OnSessionScreenControlRequested(string username)
		{
			sessionServer.SendScreenControlRsp(username);	
		}

		public void OnSessionScreenControlPermissionRequested(char[] sessionKey, string username, Boolean permission)
		{
			clientListener.OnSessionScreenControlPermissionRequested(this, sessionKey, username, permission);			
		}

		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("Client.OnSessionOperationFail");
			Console.WriteLine("errorMessage: {0}", errorMessage);
		}	
		
		public void OnSessionParticipantListUpdated(ArrayList participants)
		{
			sessionServer.SendParticipantsListRsp(participants);
		}
		
		public void OnSessionNotificationUpdate(string type, string username)
		{
			sessionServer.SendNotificationRsp(type, username);
		}
		
		public void OnSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand)
		{
			clientListener.OnSurfaceCommand(this, sessionId, surfaceCommand);
		}
		
		public void OnSendSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand)
		{
			surfaceServer.SendSurfaceCommand(sessionId, surfaceCommand);
		}
	}
}