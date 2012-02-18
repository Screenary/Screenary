using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Screenary.Server
{
	public class ScreencastingSession
	{
		public char[] sessionKey { get; set; }
		public UInt32 senderId { get; set; }
		public string senderUsername { get; set; }
		private string sessionPassword;
		public Client senderClient;
		private Client remoteController;
	
		/* Lists of TCP Clients */
		public ConcurrentDictionary<Client, UInt32> joinedClients { get; set; }
		public ConcurrentDictionary<Client, User> authenticatedClients { get; set; }
		public ConcurrentDictionary<string, Client> remoteControlRequestClients { get; set; }
		
		public struct User
		{
			public UInt32 sessionId;
			public string username;
		}
		
		public ScreencastingSession(char[] sessionKey, UInt32 senderId, string senderUsername, string sessionPassword, Client senderClient)
		{
			this.sessionKey = sessionKey;
			this.senderId = senderId;
			this.senderUsername = senderUsername;
			this.sessionPassword = sessionPassword;
			this.senderClient = senderClient;
			this.remoteController = null;
			this.joinedClients = new ConcurrentDictionary<Client, UInt32>();
			this.authenticatedClients = new ConcurrentDictionary<Client, User>();
			this.remoteControlRequestClients = new ConcurrentDictionary<string, Client>();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddJoinedUser(Client client, UInt32 id)
		{
			joinedClients.TryAdd(client, id);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddFirstUser(Client client, UInt32 id, string username)
		{			
			User user;
			user.sessionId = id;
			user.username = username;
			remoteController = client;
			authenticatedClients.TryAdd(client, user);
			senderClient = client;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddAuthenticatedUser(Client client, UInt32 id, string username)
		{			
			Boolean done = false;
			
			while (!done)
			{
				User user;
				user.sessionId = id;
				user.username = username;
				authenticatedClients.TryAdd(client, user);
				done = true;
			}
			
			UpdateNotifications("joined",username);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateAllParticipants()
		{
			foreach (Client client in authenticatedClients.Keys)
			{
				client.OnSessionParticipantListUpdated(GetParticipantUsernames());
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateNotifications(string type, string username)
		{
			foreach (Client clients in authenticatedClients.Keys)
			{
				Console.WriteLine("UpdateNotifications: {0}", username);
				clients.OnSessionNotificationUpdate(type, username);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveAuthenticatedUser(Client client, string username, UInt32 sessionId)
		{
			User user;
			authenticatedClients.TryRemove(client, out user);
			joinedClients.TryRemove(client, out sessionId);
			UpdateNotifications("left", username);
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool Authenticate(Client client, UInt32 sessionId, string username, string password)
		{			
			bool isAuthenticated = (this.sessionPassword.Equals(password));
			
			if (isAuthenticated)
			{
				this.AddAuthenticatedUser(client, sessionId, username);				
			}
	
			return isAuthenticated;
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddRemoteAccessRequest(Client requestingClient, string username)
		{
			Console.WriteLine("ScreencastingSession.AddRemoteAccessRequest");
			
			if(authenticatedClients.ContainsKey(requestingClient)) 
			{
				/*if another receiver has control, deny*/
				if(remoteController != this.senderClient)
				{
					DenyRemoteAccess(requestingClient, username);
				}
				/*if sender has control, add requester to list and inform sender*/
				else
				{
					remoteControlRequestClients.TryAdd(username, requestingClient);				
					senderClient.OnSessionRemoteAccessRequested(username);
				}
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void GrantRemoteAccess(Client senderClient, string receiverUsername)
		{
			Console.WriteLine("ScreencastingSession.GrantRemoteAccess receiverUsername: {0}", receiverUsername);

			string potentialSenderUsername = authenticatedClients[senderClient].username;
			
			if(senderUsername.Equals(potentialSenderUsername))
			{				
				Client receiverClient = remoteControlRequestClients[receiverUsername];

				if(receiverClient != null) 
				{
					remoteControlRequestClients.Clear();
					UpdateNotifications("control of", receiverUsername);
				}
			}	
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DenyRemoteAccess(Client senderClient, string receiverUsername)
		{
			Console.WriteLine("ScreencastingSession.DenyRemoteAccess");

			string username = authenticatedClients[senderClient].username;
			
			if(senderUsername.Equals(username))
			{
				Client receiverClient = null;
				remoteControlRequestClients.TryRemove(receiverUsername, out receiverClient);
				receiverClient.OnSessionNotificationUpdate("been denied control of", receiverUsername);
			}	
		}
		
		/**
		 * Sender of Receiver with remote access may terminate and restore access to Sender
		 */
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void TermRemoteAccessRequested(string username)
		{
			Console.WriteLine("ScreencastingSession.TermRemoteAccessRequested");
			remoteController = this.senderClient;
			remoteControlRequestClients.Clear();
			UpdateNotifications("control of", this.senderUsername);	
		}
						
		public ArrayList GetParticipantUsernames()
		{
			ArrayList participantUsernames = new ArrayList();
			
			foreach (User user in authenticatedClients.Values)
			{
				participantUsernames.Add(user.username);
			}
			
			return participantUsernames;
		}
		
		public void SendMouseEventToSender(UInt16 pointerFlags, int x, int y)
		{
			senderClient.SendMouseEventToSender(pointerFlags, x, y, this.senderId);
		}
		public void SendKeyboardEventToSender(UInt16 pointerFlag, UInt16 keyCode)
		{
			senderClient.SendKeyboardEventToSender(pointerFlag, keyCode, this.senderId);
		}		
		
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool isPasswordProtected()
		{
			return (!sessionPassword.Equals(""));	
		}
	}
}
