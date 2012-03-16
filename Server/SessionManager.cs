/**
 * Screenary: Real-Time Collaboration Redefined.
 * Session Manager
 *
 * Copyright 2011-2012 Terri-Anne Cambridge <tacambridge@gmail.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace Screenary.Server
{
	public class SessionManager: IClientListener
	{
		public const byte SESSION_FLAGS_PASSWORD_PROTECTED = 0x01;
		public const byte SESSION_FLAGS_NON_PASSWORD_PROTECTED = 0x02;
		
   	    private static SessionManager instance;
		static readonly object padlock = new object();
		private ConcurrentDictionary<string, ScreencastingSession> sessions;
		private static Random rnd = new Random();
		
		private char[] sessionKeyChars;
		private const int SESSION_KEY_LENGTH = 12;
		
		/**
	 	* Initialize the dictionary that will contain the list of all the screen sessions (represented by ScreencastingSession)
	 	**/
		public SessionManager()
		{
			this.sessions = new ConcurrentDictionary<string, ScreencastingSession>();
			
	 		sessionKeyChars = new char[] {
				'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
				'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
			};
		}
		
		/**
	 	* Singleton implementation
	 	**/
		public static SessionManager Instance
	    {
	    	get 
	      	{
				/* lock for multithreading support */
				lock (padlock)
            	{
		        	if (instance == null)
		         	{
		            	instance = new SessionManager();
		         	}
		         	return instance;
				}
	      	}
	   	}
		
		/**
	 	* Processes a join request by a receiver
	 	* called by SessionServer
	 	* Verifies the state of the session and adds the client to the list
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags)
		{			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				sessionId = GenerateUniqueSessionId();
				screencastSession.AddJoinedUser(client, sessionId);
				sessionStatus = 0;

				if (screencastSession.isPasswordProtected())
					sessionFlags = SESSION_FLAGS_PASSWORD_PROTECTED;
				else
					sessionFlags = SESSION_FLAGS_NON_PASSWORD_PROTECTED;
								
				return;
			}
						
			sessionStatus = 1;
		}
		
		/**
	 	* Processes a leave request by a receiver	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionLeaveRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, string username)
		{			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				if (screencastSession.authenticatedClients.ContainsKey(client))
				{
					screencastSession.RemoveAuthenticatedUser(client, username, sessionId);
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					sessionStatus = 0;

					return;
				}
			}

			sessionStatus = 1;
		}
		
		/**
	 	* Processes an authentication request	 	
	 	**/

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionAuthenticationRequested(Client client, UInt32 sessionId, char[] sessionKey, string username, string password, ref UInt32 sessionStatus)
		{
			if (isSessionAlive(sessionKey))
			{	
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				if (screencastSession.Authenticate(client, sessionId, username, password))
				{
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					screencastSession.UpdateNotifications("joined",username);
					sessionStatus = 0;
					return;
				}
			}
			
			sessionStatus = 1;
		}
		
		/**
	 	* Processes a create request by a sender	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionCreateRequested(Client client, string username, string password, ref UInt32 sessionId, ref char[] sessionKey)
		{			
			sessionId = GenerateUniqueSessionId();
			sessionKey = GenerateUniqueSessionKey();

			ScreencastingSession screencastSession = new ScreencastingSession(sessionKey, sessionId, username, password, client);
			
			sessions.TryAdd(new string(sessionKey), screencastSession);
			screencastSession.AddFirstUser(client, sessionId, username);
		}
		
		/**
	 	* Processes a terminate request by a sender	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				/* Only the sender can terminate a session */
				if (screencastSession.senderId == sessionId)
				{
					sessionStatus = 0;
					foreach(Client authenticatedClient in screencastSession.authenticatedClients.Keys)
					{
						UInt32 clientSessionId = screencastSession.authenticatedClients[authenticatedClient].sessionId;
						authenticatedClient.OnSessionTermination(clientSessionId, sessionKey, sessionStatus);
					}
					screencastSession.authenticatedClients.Clear();
					sessions.TryRemove(new string(sessionKey), out screencastSession);
					return;
				}
			}

			sessionStatus = 1;
		}	
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionRemoteAccessRequested(Client receiverClient, char[] sessionKey, string username)
		{
			ScreencastingSession screencastSession = getSessionByKey(sessionKey);
			screencastSession.AddRemoteAccessRequest(receiverClient, username);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionRemoteAccessPermissionSet(Client client, char[] sessionKey, string username, Boolean permission)
		{
			ScreencastingSession screencastSession = getSessionByKey(sessionKey);
			if(permission)
			{
				screencastSession.GrantRemoteAccess(client, username);	
			}
			else
			{
				screencastSession.DenyRemoteAccess(client, username);	
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionTermRemoteAccessRequested(Client client, char[] sessionKey, string username)
		{
			ScreencastingSession screencastSession = getSessionByKey(sessionKey);
			screencastSession.TermRemoteAccessRequested(username);
		}

		/**
	 	* Processes a modification in the participant list
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		private void OnSessionParticipantListUpdated(char[] sessionKey)
		{	
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession session = getSessionByKey(sessionKey);
				ArrayList participantUsernames = session.GetParticipantUsernames();
				
				foreach (Client client in session.authenticatedClients.Keys)
				{
					client.OnSessionParticipantListUpdated(participantUsernames);
				}
			}
		}	
				
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSurfaceCommand(Client client, UInt32 sessionId, byte[] surfaceCommand)
		{
			Console.WriteLine("SessionManager.OnSurfaceCommand");
			
			ScreencastingSession session = GetSessionBySenderId(sessionId);
			
			if (session == null)
				return;
			
			if (isSessionAlive(session.sessionKey))
			{				
				foreach (Client receiver in session.authenticatedClients.Keys)
				{
					UInt32 receiverSessionId = session.authenticatedClients[receiver].sessionId;
					
					if (receiverSessionId != sessionId)
					{
						receiver.OnSendSurfaceCommand(receiverSessionId, surfaceCommand);
					}
				}
			}	
		}
		
		/**
	 	* 	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnRecvMouseEvent(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, UInt16 pointerFlags, int x, int y)
		{			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				Console.WriteLine("ScreencastSession sender is: " + screencastSession.senderId + " " + screencastSession.senderUsername);
				screencastSession.SendMouseEventToSender(pointerFlags, x, y);
				return;
				
			}

			sessionStatus = 1;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnRecvKeyboardEvent(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, UInt16 pointerFlag, UInt16 keyCode)
		{
			Console.WriteLine("SessionManager.OnRecvKeyboardEvent sessionkey: "+sessionKey.ToString());
			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);				
				screencastSession.SendKeyboardEventToSender(pointerFlag, keyCode);
				return;
				
			}

			sessionStatus = 1;
		}
		
		/**
	 	* Generates a unique key for sessionKey
	 	**/
		private char[] GenerateUniqueSessionKey()
		{
			char[] sessionKey = new char[SESSION_KEY_LENGTH];
			
			do
			{
				for (int i = 0; i < SESSION_KEY_LENGTH; i++)
				{
					sessionKey[i] = sessionKeyChars[rnd.Next(0, sessionKeyChars.Length - 1)];
				}
			}
			while (sessions.ContainsKey(new string(sessionKey)));
			
			return sessionKey;
		}
		
		/**
	 	* Generates a unique sessionId
	 	**/
		private UInt32 GenerateUniqueSessionId()
		{
			UInt32 sessionId;
			
			/* TODO: ensure uniqueness of SessionId! */
			
			sessionId = (UInt32) rnd.Next();
			
			return sessionId;
		}
		
		private ScreencastingSession GetSessionBySenderId(UInt32 sessionId)
		{
			foreach (ScreencastingSession screencastSession in sessions.Values)
			{
				if (screencastSession.senderId == sessionId)
				{
					return screencastSession;
				}
			}
			return null;
		}
		
		private bool isSessionAlive(char[] sessionKey)
		{
			return sessions.ContainsKey(new string(sessionKey));
		}
		
		private ScreencastingSession getSessionByKey(char[] sessionKey)
		{
			return sessions[new string(sessionKey)];
		}
	}
}
