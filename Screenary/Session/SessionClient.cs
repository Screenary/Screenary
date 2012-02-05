using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SessionClient : SessionChannel
	{
		protected UInt32 sessionId;
		private ISessionResponseListener listener;
		private readonly object channelLock = new object();
		static private bool stopthread = false;
		
		public SessionClient(TransportClient transport, ISessionResponseListener listener)
		{
			this.transport = transport;
			this.listener = listener;
			this.sessionId = 0;			
		}
		
		public UInt32 GetSessionId()
		{
			return sessionId;
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		/**
	 	* Sends a join request to the server. A sessionKey identifies the session to join.
	 	* Used by receiver
	 	**/
		public void SendJoinReq(char[] sessionKey)
		{
			Console.WriteLine("SessionClient.SendJoinReq");
			
			UInt32 sessionId = 0;
			
			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write(sessionKey);
						
			Send(buffer, PDU_SESSION_JOIN_REQ);
		}
		
		/**
	 	* Sends a leave request to the server. The sessionId returned after a successful join request 
	 	* should be passed with the username. User by receiver.
	 	**/
		public void SendLeaveReq(string username)
		{
			Console.WriteLine("SessionClient.SendLeaveReq");
						
			byte[] buffer = null;
			int length = username.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
			
			s.Write((UInt16) username.Length);
			s.Write(username.ToCharArray());
									
			Send(buffer, PDU_SESSION_LEAVE_REQ);
		}
		
		/**
	 	* Sends an authentication request to the server. The username, password and sessionId should be sent 
	 	* with the request.
	 	**/
		public void SendAuthReq(string username, string password)
		{
			Console.WriteLine("SessionClient.SendAuthReq");
						
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
						
			Send(buffer, PDU_SESSION_AUTH_REQ);
		}
		
		/**
	 	* Sends a create request to the server. Should be used by senders.
	 	**/
		public void SendCreateReq(string username, string password)
		{
			Console.WriteLine("SessionClient.SendCreateReq");

			UInt32 sessionId = 0;
			
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
				
			Send(buffer, PDU_SESSION_CREATE_REQ);
		}
		
		/**
	 	* Allows a sender to send a terminate request.
	 	**/
		public void SendTermReq(char[] sessionKey)
		{
			Console.WriteLine("SessionClient.SendTermReq");
						
			byte[] buffer = null;
			int length = sessionKey.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_TERM_REQ);
			Console.WriteLine("SessionClient.SendTermReq done");
		}
				
		/**
	 	* Processes a join response
	 	**/
		public void RecvJoinRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvJoinRsp");

			Boolean isPasswordProtected = false;
			UInt32 sessionStatus;
			char[] sessionKey;
			byte sessionFlags;
			
			this.sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Join Failed: {0}", sessionStatus);
				listener.OnSessionOperationFail("Session Join Failed");
				return;
			}
			
			sessionKey = s.ReadChars(12);
			sessionFlags = s.ReadByte();
			
			if (sessionFlags == SESSION_FLAGS_PASSWORD_PROTECTED)
			{
				isPasswordProtected = true;
			}
			
			listener.OnSessionJoinSuccess(sessionKey, isPasswordProtected);
		}
		
		/**
	 	* Processes a leave response
	 	**/
		public void RecvLeaveRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvLeaveRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0 || sessionId != this.sessionId)
			{
				Console.WriteLine("Session Leave Failed: {0}", sessionStatus);
				listener.OnSessionOperationFail("Session Leave Failed");
				return;
			}
			
			listener.OnSessionLeaveSuccess();
		}
		
		/**
	 	* Processes an authentication response
	 	**/
		public void RecvAuthRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvAuthRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Authentication Failed: {0}", sessionStatus);
				listener.OnSessionOperationFail("Session Authentication Failed");
				return;
			}
			else if (sessionId != this.sessionId)
			{
				Console.WriteLine("Session Authentication Failed:" + sessionId + "!=" + this.sessionId);
				listener.OnSessionOperationFail("Session Authentication Failed");
				return;
			}
			
			listener.OnSessionAuthenticationSuccess();
		}
		
		/**
	 	* Processes a create session response
	 	**/
		public void RecvCreateRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvCreateRsp");
			
			UInt32 sessionStatus;
			char[] sessionKey;
			
			this.sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				listener.OnSessionOperationFail("Session Creation Failed");
				return;
			}
			
			sessionKey = s.ReadChars(12);
							
			listener.OnSessionCreationSuccess(sessionKey);
		}
		
		/**
	 	* Processes a terminate session response
	 	**/
		public void RecvTermRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvTermRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0 || sessionId != this.sessionId)
			{
				Console.WriteLine("Session Termination Failed: {0}", sessionStatus);
				listener.OnSessionOperationFail("Session Termination Failed");
				return;
			}
			
			sessionKey = s.ReadChars(12);
			
			listener.OnSessionTerminationSuccess(sessionKey);
		}
		
		/**
	 	* Processes the received list of participants
	 	**/
		public void RecvParticipantListRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvPartipantsListRsp");
			
			ArrayList participants = new ArrayList();
			
			int length = (int) s.ReadUInt16();
			
			/* subtract bytes stored for total length */
			length -= 2;
			
			while(length > 0)
			{
				string username = "";

				UInt16 usernameLength = s.ReadUInt16();
				
				if (usernameLength > 0)
					username = new string(s.ReadChars(usernameLength));
								
				participants.Add(username);
				
				/* subtract bytes stored for length and string */
				length -= (username.Length + 2);
			}
			
			listener.OnSessionParticipantListUpdate(participants);
		}
		
		/**
	 	* Processes notification update
	 	**/
		public void RecvNotificationUpdate(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvNotificationUpdate");
			
			string type = "";
			string username = "";
			
			int length = (int) s.ReadUInt16();
			
			/* subtract bytes stored for total length */
			length -= 2;
			
			while(length > 0)
			{
				UInt16 typeLength = s.ReadUInt16();
				
				if (typeLength > 0)
					type = new string(s.ReadChars(typeLength));
				
				/* subtract bytes stored for length and string */
				length -= (type.Length + 2);
				
				UInt16 usernameLength = s.ReadUInt16();
				
				if (usernameLength > 0)
					username = new string(s.ReadChars(usernameLength));
				
				/* subtract bytes stored for length and string */
				length -= (username.Length + 2);
			}
			
			listener.OnSessionNotificationUpdate(type, username);
		}
		
		public override void OnRecv(byte[] buffer, byte pduType)
		{
			lock (channelLock)
			{
				queue.Enqueue(new PDU(buffer, GetChannelId(), pduType));
				Monitor.Pulse(channelLock);
			}
		}
		
		public override void OnOpen()
		{
			thread = new Thread(ChannelThreadProc);
			thread.Start();
		}
		
		/**
	 	* Called by the dispatcher when the application is disconneting
	 	* The thread should be stopped.
	 	**/
		public override void OnClose()
		{
			lock (channelLock)
			{
				stopthread = true;
				Console.WriteLine("closing channel: " + this.ToString());
				Monitor.PulseAll(channelLock);
			}
		}
		
		/**
	 	* Processes a received PDU and calls the appropriate handler
	 	**/
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_SESSION_JOIN_RSP:
					RecvJoinRsp(s);
					return;
				
				case PDU_SESSION_LEAVE_RSP:
					RecvLeaveRsp(s);
					return;
				
				case PDU_SESSION_CREATE_RSP:
					RecvCreateRsp(s);
					return;
				
				case PDU_SESSION_TERM_RSP:
					RecvTermRsp(s);
					return;
				
				case PDU_SESSION_AUTH_RSP:
					RecvAuthRsp(s);
					return;
				
				case PDU_SESSION_PARTICIPANTS_RSP:
					RecvParticipantListRsp(s);
					return;
				
			case PDU_SESSION_NOTIFICATION_RSP:
					RecvNotificationUpdate(s);
					return;
					
				default:
					return;
			}
		}
		
		/**
	 	* Code executed by the thread. Listening and processing received packets
	 	**/
		public void ChannelThreadProc()
		{
			Console.WriteLine("SessionClient.ChannelThreadProc");
			
			while (!stopthread)
			{
				lock (channelLock)
				{
					while (queue.Count < 1 && !stopthread)
					{
						Monitor.Wait(channelLock);
					}

					if (queue.Count >= 1)
					{
						PDU pdu = (PDU) queue.Dequeue();
						ProcessPDU(pdu.Buffer, pdu.Type);
					}
					
					Monitor.Pulse(channelLock);
				}
			}
			
			Console.WriteLine("SessionClient.ChannelThreadProc end");
		}
	}
}

