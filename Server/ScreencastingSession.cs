using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Screenary.Server
{
	public class ScreencastingSession
	{
		public char[] sessionKey {get; set;}
		public UInt32 senderId {get; set;}
		public string senderUsername {get; set;}
		private string sessionPassword;
		
		/* Lists of TCP Clients */
		public ConcurrentDictionary<Client, UInt32> joinedClients {get; set;}
		public ConcurrentDictionary<Client, User> authenticatedClients {get; set;}
		
		public struct User
		{
			public UInt32 sessionId;
			public string username;
		}
		
		public ScreencastingSession(char[] sessionKey, UInt32 senderId, string senderUsername, string sessionPassword)
		{
			this.sessionKey = sessionKey;
			this.senderId = senderId;
			this.senderUsername = senderUsername;
			this.sessionPassword = sessionPassword;
			this.joinedClients = new ConcurrentDictionary<Client, UInt32>();
			this.authenticatedClients = new ConcurrentDictionary<Client, User>();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddJoinedUser(Client client, UInt32 id)
		{
			joinedClients.TryAdd(client, id);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddFirstUser(Client client, UInt32 id, string username)
		{
			//joinedClients.Remove(client);
			
			User user;
			user.sessionId = id;
			user.username = username;
			authenticatedClients.TryAdd(client, user);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddAuthenticatedUser(Client client, UInt32 id, string username)
		{
			//joinedClients.Remove(client);
			
			Boolean done = false;
			
			while(!done)
			{
				User user;
				user.sessionId = id;
				user.username = username;
				authenticatedClients.TryAdd(client, user);
				done = true;
			}
			
			UpdateNotifications(client, "joined",username);
		}
				
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateAllParticipants()
		{
			foreach(Client client in authenticatedClients.Keys)
			{
				client.OnSessionParticipantListUpdated(GetParticipantUsernames());
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateNotifications(Client client, string type, string username)
		{
			foreach(Client clients in authenticatedClients.Keys)
			{
				clients.OnSessionNotificationUpdate(type,username);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveAuthenticatedUser(Client client, string username, UInt32 sessionId)
		{
			User user;
			authenticatedClients.TryRemove(client, out user);
			joinedClients.TryRemove(client, out sessionId);
			UpdateNotifications(client, "left", username);
		}		
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public Boolean Authenticate(Client client, UInt32 sessionId, string username, string password)
		{
			Boolean isAuthenticated = (this.sessionPassword == password);
			if(isAuthenticated)
			{
				this.AddAuthenticatedUser(client, sessionId, username);				
			}
	
			return isAuthenticated;
		}
		
		public ArrayList GetParticipantUsernames()
		{
			ArrayList participantUsernames = new ArrayList();
			foreach(User user in authenticatedClients.Values)
			{
				participantUsernames.Add(user.username);
			}
			return participantUsernames;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool isPasswordProtected()
		{
			return (!sessionPassword.Equals(""));	
		}
		
	}
}
