/**
 * Screenary: Real-Time Collaboration Redefined.
 * Client
 *
 * Copyright 2011-2012 Terri-Anne Cambridge <tacambridge@gmail.com>
 * Copyright 2011-2012 Marwan Samaha <mar6@hotmail.com>
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
		}	
		
		public void OnSessionTermination(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			sessionServer.SendTermRsp(sessionId, sessionKey, sessionStatus);
		}	

		/**
		 * Receiver sends request to Broadcaster
		 */
		public void OnSessionRemoteAccessRequested(char[] sessionKey, string username)
		{
			clientListener.OnSessionRemoteAccessRequested(this, sessionKey, username);
		}
		
		/**
		 * Broadcaster informs Sender of the request
		 */
		public void OnSessionRemoteAccessRequested(string username)
		{
			sessionServer.SendRemoteAccessRsp(username);	
		}

		public void OnSessionRemoteAccessPermissionSet(char[] sessionKey, string username, Boolean permission)
		{
			clientListener.OnSessionRemoteAccessPermissionSet(this, sessionKey, username, permission);			
		}

		public void OnSessionTermRemoteAccessRequested(char[] sessionKey, string username)
		{
			clientListener.OnSessionTermRemoteAccessRequested(this, sessionKey, username);			
		}
				
		public void OnSessionParticipantListUpdated(ArrayList participants)
		{
			sessionServer.SendParticipantsListRsp(participants);
		}
		
		public void OnSessionNotificationUpdate(string type, string username)
		{
			sessionServer.SendNotificationRsp(type, username);
		}
		
		public void OnSessionFirstNotificationUpdate(string type, string username, string senderClient)
		{
			sessionServer.SendFirstNotificationRsp(type, username, senderClient);
		}
		
		public void OnSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand)
		{
			clientListener.OnSurfaceCommand(this, sessionId, surfaceCommand);
		}
		
		public void OnSendSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand)
		{
			surfaceServer.SendSurfaceCommand(sessionId, surfaceCommand);
		}
		
		public void SendMouseEventToSender(UInt16 pointerFlag, int x, int y, UInt32 sessionId)
		{
			inputServer.SendMouseEventToSender(pointerFlag, x, y, sessionId);
		}
		public void SendKeyboardEventToSender(UInt16 pointerFlag, UInt16 keyCode, UInt32 sessionId)
		{
			inputServer.SendKeyboardEventToSender(pointerFlag, keyCode, sessionId);
		}
		
		public void OnRecvMouseEvent(UInt32 sessionId, UInt16 pointerFlag, int x, int y)
		{
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnRecvMouseEvent(this, sessionId, sessionServer.sessionKey, ref sessionStatus, pointerFlag, x, y);	
		}
		public void OnRecvKeyboardEvent(UInt32 sessionId, UInt16 pointerFlag, UInt16 keyCode)
		{
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientListener.OnRecvKeyboardEvent(this, sessionId, sessionServer.sessionKey, ref sessionStatus, pointerFlag, keyCode);
		}
		
	}
}