using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace Screenary.Server
{
	public class ScreenSessions: IClientRequestListener
	{
		public const byte SESSION_FLAGS_PASSWORD_PROTECTED = 0x01;
		public const byte SESSION_FLAGS_NON_PASSWORD_PROTECTED = 0x02;
		
   	    private static ScreenSessions instance;
		static readonly object padlock = new object();
		private ConcurrentDictionary<string, ScreencastingSession> sessions; 
		private static Random rnd = new Random();
		private string username;
		
		public ScreenSessions ()
		{
			this.sessions = new ConcurrentDictionary<string, ScreencastingSession>();
		}
		
		public static ScreenSessions Instance
	    {
	    	get 
	      	{
				//lock for multithreading support
				lock (padlock)
            	{
		        	if (instance == null)
		         	{
		            	instance = new ScreenSessions();
		         	}
		         	return instance;
				}
	      	}
	   	}
		
		public void addPDU(PDU pdu, char[] sessionKey)
		{
			string sessionKeyString = new string(sessionKey);
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession session = sessions[sessionKeyString];
				foreach (Client client in session.authenticatedClients.Keys)
				{
					client.addPDU(pdu);
				}
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags)
		{
			Console.WriteLine("ScreenSessions.OnSessionJoinRequested");
			
			string sessionKeyString = new string(sessionKey);
			
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				sessionId = GenerateUniqueId();
				screencastSession.AddJoinedUser(client, sessionId);
				sessionStatus = 0;

				if(screencastSession.isPasswordProtected())
					sessionFlags = SESSION_FLAGS_PASSWORD_PROTECTED;
				else
					sessionFlags = SESSION_FLAGS_NON_PASSWORD_PROTECTED;
				
				Console.WriteLine("SessionKey:{0}, SessionId:{1}, SessionStatus:{2}", 
						sessionKeyString, sessionId, sessionStatus);
				
				return;
			}
						
			sessionStatus = 1;
			
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionLeaveRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionLeaveRequested");
			
			string sessionKeyString = new string(sessionKey);
			
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				if(screencastSession.authenticatedClients.ContainsKey(client))
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionAuthenticationRequested(Client client, UInt32 sessionId, char[] sessionKey, string username, string password, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			string sessionKeyString = new string(sessionKey);
			this.username = username;
			
			if(isSessionAlive(sessionKeyString))
			{	
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				if(screencastSession.Authenticate(client, sessionId, username, password))
				{
					OnSessionParticipantListUpdated(screencastSession.sessionKey);
					screencastSession.UpdateNotifications("joined",username);
					sessionStatus = 0;
					return;
				}
			}
			
			sessionStatus = 1;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionCreateRequested(Client client, string username, string password, ref UInt32 sessionId, ref char[] sessionKey)
		{
			Console.WriteLine("ScreenSessions.OnSessionCreateRequested");
			
			sessionId = GenerateUniqueId();
			sessionKey = GenerateUniqueKey();
			string sessionKeyString = new string(sessionKey);

			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);

			ScreencastingSession screencastSession = new ScreencastingSession(sessionKey, sessionId, username, password);
			
			sessions.TryAdd(sessionKeyString, screencastSession);
			screencastSession.AddFirstUser(client, sessionId, username);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionTerminationRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);
			
			if(isSessionAlive(sessionKeyString))
			{
				/*Only the sender can terminate a session*/
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				if(screencastSession.senderId == sessionId)
				{
					screencastSession.authenticatedClients.Clear();
					sessions.TryRemove(sessionKeyString, out screencastSession);
					sessionStatus = 0;
					return;
				}
			}

			sessionStatus = 1;
		}	
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		private void OnSessionParticipantListUpdated(char[] sessionKey)
		{
			Console.WriteLine("ScreenSessions.OnSessionParticipantsListUpdated");
			string sessionKeyString = new string(sessionKey);
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession session = sessions[sessionKeyString];
				ArrayList participantUsernames = session.GetParticipantUsernames();
				foreach(Client client in session.authenticatedClients.Keys)
				{
					client.OnSessionParticipantListUpdated(participantUsernames);				}
				}
		}	
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("ScreenSessions.OnSessionOperationFail");
		}	
		
		private char[] GenerateUniqueKey()
		{
			string attemptSessionKey = System.Guid.NewGuid().ToString().Substring(0,12);			
			while(sessions.ContainsKey(attemptSessionKey))
			{
				attemptSessionKey = System.Guid.NewGuid().ToString().Substring(0,12);
			}
			
			char[] sessionKey = attemptSessionKey.ToCharArray();
			
			return sessionKey;
		}
		
		private UInt32 GenerateUniqueId()
		{
			return (UInt32) rnd.Next();
		}
		
		private ScreencastingSession GetBySenderSessionId(UInt32 sessionId)
		{
			foreach(ScreencastingSession screencastSession in sessions.Values)
			{
				if(screencastSession.senderId == sessionId)
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
		
	}
}