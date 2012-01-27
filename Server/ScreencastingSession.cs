using System;
using System.Collections;
using System.Collections.Generic;

namespace Screenary.Server
{
	public class ScreencastingSession
	{
		public char[] sessionKey {get; set;}
		public UInt32 senderId {get; set;}
		public string senderUsername {get; set;}
		private string sessionPassword;
		
		/* Lists of TCP Clients */
		public Dictionary<Client, UInt32> joinedClients {get; set;}
		public Dictionary<Client, User> authenticatedClients {get; set;}
		
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
			joinedClients = new Dictionary<Client, UInt32>();
			authenticatedClients = new Dictionary<Client, User>();
		}

		public void AddJoinedUser(Client client, UInt32 id)
		{
			joinedClients.Add(client, id);
		}
		
		public void AddFirstUser(Client client, UInt32 id, string username)
		{
			//joinedClients.Remove(client);
			
			User user;
			user.receiverId = id;
			user.receiverUsername = username;
			authenticatedClients.Add(client, user);
		}
		
		public void AddAuthenticatedUser(Client client, UInt32 id, string username)
		{
			//joinedClients.Remove(client);
			
			Boolean done = false;
			
			while(!done)
			{
				User user;
				user.receiverId = id;
				user.receiverUsername = username;
				authenticatedClients.Add(client, user);
				done = true;
			}
			
			UpdateNotifications("joined",username);
		}
		
	
		
		public void UpdateAllParticipants()
		{
			foreach(Client client in authenticatedClients.Keys)
			{
				client.OnSessionParticipantListUpdated(GetParticipantUsernames());
			}
		}
		
		public void UpdateNotifications(string type, string username)
		{
			foreach(Client client in authenticatedClients.Keys)
			{
				client.OnSessionNotificationUpdate(type,username);
			}
		}

		public void RemoveAuthenticatedUser(Client client, string username)
		{
			authenticatedClients.Remove(client);
			UpdateNotifications("left", username);
		}		
		
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
		
		public bool isPasswordProtected()
		{
			return (!sessionPassword.Equals(""));	
		}
		
	}
}
