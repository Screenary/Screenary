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
			Console.WriteLine("ScreenSessions.OnSessionJoinRequested");
			
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
				
				Console.WriteLine("SessionKey:{0}, SessionId:{1}, SessionStatus:{2}", 
						new string(sessionKey), sessionId, sessionStatus);
				
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
			Console.WriteLine("ScreenSessions.OnSessionLeaveRequested");
			
			if (isSessionAlive(sessionKey))
			{
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				if (screencastSession.authenticatedClients.ContainsKey(client))
				{
					screencastSession.RemoveAuthenticatedUser(client, username, sessionId);
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					sessionStatus = 0;

					Console.WriteLine("SessionKey:{0}, SessionId:{1}, SessionStatus:{2}", 
						new string(sessionKey), sessionId, sessionStatus);

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
			Console.WriteLine("ScreenSessions.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			if (isSessionAlive(sessionKey))
			{	
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				if (screencastSession.Authenticate(client, sessionId, username, password))
				{
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					screencastSession.UpdateNotifications(client, "joined",username);
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
			Console.WriteLine("ScreenSessions.OnSessionCreateRequested");
			
			sessionId = GenerateUniqueSessionId();
			sessionKey = GenerateUniqueSessionKey();

			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);

			ScreencastingSession screencastSession = new ScreencastingSession(sessionKey, sessionId, username, password);
			
			sessions.TryAdd(new string(sessionKey), screencastSession);
			screencastSession.AddFirstUser(client, sessionId, username);
		}
		
		/**
	 	* Processes a terminate request by a sender	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionTerminationRequested");
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, new string(sessionKey));
			
			if (isSessionAlive(sessionKey))
			{
				/* Only the sender can terminate a session */
				ScreencastingSession screencastSession = getSessionByKey(sessionKey);
				
				if (screencastSession.senderId == sessionId)
				{
					screencastSession.authenticatedClients.Clear();
					sessions.TryRemove(new string(sessionKey), out screencastSession);
					sessionStatus = 0;
					return;
				}
			}

			sessionStatus = 1;
		}	
		
		/**
	 	* Processes a modification in the participant list
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		private void OnSessionParticipantListUpdated(char[] sessionKey)
		{
			Console.WriteLine("ScreenSessions.OnSessionParticipantsListUpdated");
			
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
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("ScreenSessions.OnSessionOperationFail");
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
