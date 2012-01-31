using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Screenary.Server
{
	public class ScreenSessions: IClientRequestListener
	{
		
   	    private static ScreenSessions instance;
		static readonly object padlock = new object();
		private Dictionary<string, ScreencastingSession> sessions; 
		private static UInt32 sessionId = 100;
		
		public ScreenSessions ()
		{
			this.sessions = new Dictionary<string, ScreencastingSession>();
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
		
		public void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags)
		{
			Console.WriteLine("ScreenSessions.OnSessionJoinRequested");
			
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionKey:{0}", sessionKeyString);
			
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				sessionId = GenerateUniqueId();
				screencastSession.AddJoinedUser(client, sessionId);
				sessionStatus = 0;
				return;
			}
						
			sessionStatus = 1;
			
		}
		
		public void OnSessionLeaveRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionLeaveRequested");
			
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("sessionId:{0} sessionKey:{1} ", sessionId, sessionKeyString);
			
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				if(screencastSession.authenticatedClients.ContainsKey(client))
				{
					screencastSession.RemoveAuthenticatedUser(client);
					OnSessionPartipantListUpdated(screencastSession.sessionKey);
					sessionStatus = 0;
					return;
				}
			}

			sessionStatus = 1;
		}

		public void OnSessionAuthenticationRequested(Client client, UInt32 sessionId, char[] sessionKey, string username, string password, ref UInt32 sessionStatus)
		{
			Console.WriteLine("ScreenSessions.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			string sessionKeyString = new string(sessionKey);
			
			if(isSessionAlive(sessionKeyString))
			{	
				ScreencastingSession screencastSession = sessions[sessionKeyString];
				if(screencastSession.Authenticate(client, sessionId, username, password))
				{
					OnSessionPartipantListUpdated(screencastSession.sessionKey);
					sessionStatus = 0;
					return;
				}
			}
			
			sessionStatus = 1;
		}
		
		public void OnSessionCreateRequested(Client client, string username, string password, ref UInt32 sessionId, ref char[] sessionKey)
		{
			Console.WriteLine("ScreenSessions.OnSessionCreateRequested");
			Console.WriteLine("username:{0} password:{1}", username, password);
			
			sessionId = GenerateUniqueId();
			sessionKey = GenerateUniqueKey();
			string sessionKeyString = new string(sessionKey);

			ScreencastingSession screencastSession = new ScreencastingSession(sessionKey, sessionId, username, password);
			
			sessions.Add(sessionKeyString, screencastSession);
			screencastSession.AddAuthenticatedUser(client, sessionId, username);

		}
		
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
					sessions.Remove(sessionKeyString);
					screencastSession.authenticatedClients.Clear();
					sessionStatus = 0;
					return;
				}
			}

			sessionStatus = 1;
		}	
		
		private void OnSessionPartipantListUpdated(char[] sessionKey)
		{
			Console.WriteLine("ScreenSessions.OnSessionPartipantsListSuccess");
			string sessionKeyString = new string(sessionKey);
			if(isSessionAlive(sessionKeyString))
			{
				ScreencastingSession session = sessions[sessionKeyString];
				ArrayList participantUsernames = session.GetParticipantUsernames();
				foreach(Client client in session.authenticatedClients.Keys)
				{
					client.OnSessionPartipantListUpdated(participantUsernames);
				}
			}
		}	
		
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("ScreenSessions.OnSessionOperationFail");
		}	
		
		private char[] GenerateUniqueKey()
		{
			/*
			string path = Path.GetRandomFileName(); //TODO This does not work for me. It did before but I think I messed something up (@Mar from TA)
			string attemptSessionKey = path.Replace(".", "").Substring(0, 12).ToUpperInvariant();
			char[] sessionKey = null;
			while(sessions.ContainsKey(attemptSessionKey))
			{
				sessionKey = attemptSessionKey.ToCharArray();
			}*/
			
			char[] sessionKey = "ABCDEF123456".ToCharArray();
			
			return sessionKey;
		}
		
		private UInt32 GenerateUniqueId()
		{
			return sessionId++;
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