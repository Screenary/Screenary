/**
 * Screenary: Real-Time Collaboration Redefined.
 * Screencasting Session
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
			
			UpdateFirstNotifications("joined",username, senderUsername);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateAllParticipants()
		{
			foreach (Client client in authenticatedClients.Keys)
			{
				try
				{
					client.OnSessionParticipantListUpdated(GetParticipantUsernames());
				}
				catch (TransportException e)
				{
					Console.WriteLine("Caught Transport Exception: " + e.Message);
					RemoveAuthenticatedUser(client);
				}
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateNotifications(string type, string username)
		{
			foreach (Client client in authenticatedClients.Keys)
			{
				try
				{
					client.OnSessionNotificationUpdate(type, username);
				}
				catch (TransportException e)
				{
					Console.WriteLine("Caught Transport Exception: " + e.Message);
					RemoveAuthenticatedUser(client);
				}
			}
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateFirstNotifications(string type, string username, string senderClient)
		{
			foreach (Client client in authenticatedClients.Keys)
			{
				try
				{
					client.OnSessionFirstNotificationUpdate(type, username, senderClient);
				}
				catch (TransportException e)
				{
					Console.WriteLine("Caught Transport Exception: " + e.Message);
					RemoveAuthenticatedUser(client);
				}
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
		public void RemoveAuthenticatedUser(Client client)
		{
			User user;
			UInt32 sessionId;
			authenticatedClients.TryRemove(client, out user);
			joinedClients.TryRemove(client, out sessionId);
			UpdateNotifications("left", user.username);
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
			if(authenticatedClients.ContainsKey(requestingClient)) 
			{
				/*if another receiver has control, deny*/
				if(remoteController != this.senderClient)
				{
					remoteControlRequestClients.TryAdd(username, requestingClient);				
					DenyRemoteAccess(this.senderClient, username);
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
			string potentialSenderUsername = authenticatedClients[senderClient].username;
			
			if(senderUsername.Equals(potentialSenderUsername))
			{				
				Client receiverClient = remoteControlRequestClients[receiverUsername];

				if(receiverClient != null) 
				{
					remoteControlRequestClients.TryRemove(receiverUsername, out receiverClient);
					remoteController = receiverClient;
					UpdateNotifications("control of", receiverUsername);
				}
			}	
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DenyRemoteAccess(Client senderClient, string receiverUsername)
		{			
			if(this.senderClient == senderClient)
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
