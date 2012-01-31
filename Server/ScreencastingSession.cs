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
			public UInt32 receiverId;
			public string receiverUsername;
		}
		
		public ScreencastingSession(char[] sessionKey, UInt32 senderId, string senderUsername, string sessionPassword)
		{
			this.sessionKey = sessionKey;
			this.senderId = senderId;
			this.senderUsername = senderUsername;
			this.sessionPassword = sessionPassword;
			joinedClients = new ConcurrentDictionary<Client, UInt32>();
			authenticatedClients = new ConcurrentDictionary<Client, User>();
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
			user.receiverId = id;
			user.receiverUsername = username;
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
				user.receiverId = id;
				user.receiverUsername = username;
				authenticatedClients.TryAdd(client, user);
				done = true;
			}
			
			UpdateNotifications("joined",username);
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
		public void UpdateNotifications(string type, string username)
		{
			foreach(Client client in authenticatedClients.Keys)
			{
				client.OnSessionNotificationUpdate(type,username);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveAuthenticatedUser(Client client, string username)
		{
			User user;
			authenticatedClients.TryRemove(client, out user);
			UpdateNotifications("left", username);
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
				participantUsernames.Add(user.receiverUsername);
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
