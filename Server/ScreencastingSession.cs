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
		private Client screenController;
	
		/* Lists of TCP Clients */
		public ConcurrentDictionary<Client, UInt32> joinedClients { get; set; }
		public ConcurrentDictionary<Client, User> authenticatedClients { get; set; }
		public ConcurrentDictionary<string, Client> screenControlRequestClients { get; set; } //TA TODO What if username is not unique? maybe pass around sessionId instead
		
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
			this.screenController = null;
			this.joinedClients = new ConcurrentDictionary<Client, UInt32>();
			this.authenticatedClients = new ConcurrentDictionary<Client, User>();
			this.screenControlRequestClients = new ConcurrentDictionary<string, Client>();
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
			screenController = client;
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
		public void AddScreenControlRequest(Client requestingClient, string username)
		{
			Console.WriteLine("ScreencastingSession.AddScreenControlRequest");
			
			if(authenticatedClients.ContainsKey(requestingClient)) 
			{
				/*if another receiver has control, deny*/
				if(screenController != senderClient)
				{
					DenyScreenControl(requestingClient, username);
				}
				/*if sender is requesting control, regain it*/
				else if(senderClient == requestingClient)
				{
					screenController = requestingClient;
					UpdateNotifications("control of", username);					
				}
				/*if sender has control, add requester to list and inform sender*/
				else
				{
					Console.WriteLine("ScreencastingSession.AddScreenControlRequest /*if sender has control, add requester to list and inform sender*/");
					screenControlRequestClients.TryAdd(username, requestingClient);				
					senderClient.OnSessionScreenControlRequested(username);
				}
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void GrantScreenControl(Client senderClient, string receiverUsername)
		{
			Console.WriteLine("ScreencastingSession.GrantScreenControl receiverUsername: {0}", receiverUsername);

			string potentialSenderUsername = authenticatedClients[senderClient].username;
			
			if(senderUsername.Equals(potentialSenderUsername))
			{
				Console.WriteLine("ScreencastingSession.GrantScreenControl senderUsername.Equals(potentialSenderUsername) is TRUE");
				
				Client receiverClient = screenControlRequestClients[receiverUsername];

				if(receiverClient != null) 
				{
					Console.WriteLine("ScreencastingSession.GrantScreenControl receiverClient is not null");
					screenController = receiverClient;
					screenControlRequestClients.TryRemove(receiverUsername, out receiverClient);
					UpdateNotifications("control of", receiverUsername);
				}
			}	
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DenyScreenControl(Client senderClient, string receiverUsername)
		{
			Console.WriteLine("ScreencastingSession.DenyScreenControl");

			string username = authenticatedClients[senderClient].username;
			
			if(senderUsername.Equals(username))
			{
				Client receiverClient = null;
				screenControlRequestClients.TryRemove(receiverUsername, out receiverClient);
			}	
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
