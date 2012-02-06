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
		
		/*
		//TA: TEST if when a receiver joins after the start of the session, he will also get updates
		public void addPDU(PDU pdu, char[] sessionKey)
		{
			string sessionKeyString = new string(sessionKey);
			
			if (isSessionAlive(sessionKeyString))
			{
				ScreencastingSession session = sessions[sessionKeyString];
				
				UInt32 senderSessionId = session.senderId;
				
				foreach (Client client in session.authenticatedClients.Keys)
				{
					//if (session.authenticatedClients[client].sessionId != senderSessionId)
						//client.addPDU(pdu);
				}
			}
		}*/
		
		/**
	 	* Processes a join request by a receiver
	 	* called by SessionServer
	 	* Verifies the state of the session and adds the client to the list
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags)
		{
			Console.WriteLine("ScreenSessions.OnSessionJoinRequested");
			
			string sessionKeyString = new string(sessionKey);
			
			if (isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				sessionId = GenerateUniqueSessionId();
				screencastSession.AddJoinedUser(client, sessionId);
				sessionStatus = 0;

				if (screencastSession.isPasswordProtected())
					sessionFlags = SESSION_FLAGS_PASSWORD_PROTECTED;
				else
					sessionFlags = SESSION_FLAGS_NON_PASSWORD_PROTECTED;
				
				Console.WriteLine("SessionKey:{0}, SessionId:{1}, SessionStatus:{2}", 
						sessionKeyString, sessionId, sessionStatus);
				
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
			
			string sessionKeyString = new string(sessionKey);
			
			if (isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				
				if (screencastSession.authenticatedClients.ContainsKey(client))
				{
					screencastSession.RemoveAuthenticatedUser(client, username, sessionId);
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					sessionStatus = 0;

					Console.WriteLine("SessionKey:{0}, SessionId:{1}, SessionStatus:{2}", 
						sessionKeyString, sessionId, sessionStatus);

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
			
			string sessionKeyString = new string(sessionKey);
			
			if (isSessionAlive(sessionKeyString))
			{	
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				
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
			string sessionKeyString = new string(sessionKey);

			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);

			ScreencastingSession screencastSession = new ScreencastingSession(sessionKey, sessionId, username, password);
			
			sessions.TryAdd(sessionKeyString, screencastSession);
			screencastSession.AddFirstUser(client, sessionId, username);
		}
		
		/**
	 	* Processes a terminate request by a sender	 	
	 	**/
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionTerminationRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);
			
			if (isSessionAlive(sessionKeyString))
			{
				/* Only the sender can terminate a session */
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				
				if (screencastSession.senderId == sessionId)
				{
					screencastSession.authenticatedClients.Clear();
					sessions.TryRemove(sessionKeyString, out screencastSession);
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
			string sessionKeyString = new string(sessionKey);
			
			if (isSessionAlive(sessionKeyString))
			{
				ScreencastingSession session = sessions[sessionKeyString];
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
			while (sessions.ContainsKey(sessionKey.ToString()));
			
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
		
		private ScreencastingSession GetBySenderSessionId(UInt32 sessionId)
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
		
		private Boolean isSessionAlive(string sessionKeyString)
		{
			return sessions.ContainsKey(sessionKeyString);
		}
		
		public void OnSurfaceCommand(Client client, char[] sessionKey, byte[] buffer)
		{
			Console.WriteLine("SessionManager.OnSurfaceCommand");
		}
	}
}