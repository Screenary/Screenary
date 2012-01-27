using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{	
	public class SessionServer : SessionChannel
	{
		public char[] sessionKey {get;set;} //TODO TA privacy issue?
		private ISessionRequestListener listener;
		private readonly object channelLock = new object();
				
		public SessionServer(TransportClient transport, ISessionRequestListener listener)
		{
			this.transport = transport;
			this.listener = listener;
		}
		
		private BinaryWriter InitRspPDU(ref byte[] buffer, int length, UInt32 id, UInt32 status)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 8];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) id);
			s.Write((UInt32) status);
	
			return s;
		}
		
		public void SendJoinRsp(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus, byte sessionFlags)
		{
			Console.WriteLine("SessionServer.SendJoinRsp");
									
			this.sessionKey = sessionKey;

			byte[] buffer = null;
			int length = sessionKey.Length + 1;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			s.Write(sessionKey);
			s.Write(sessionFlags);

			Send(buffer, PDU_SESSION_JOIN_RSP);
		}
		
		public void SendLeaveRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionServer.SendLeaveRsp");
									
			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			Send(buffer, PDU_SESSION_LEAVE_RSP);
		}
		
		public void SendAuthRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionServer.SendAuthRsp");
			
			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
									
			Send(buffer, PDU_SESSION_AUTH_RSP);			
		}
		
		public void SendCreateRsp(UInt32 sessionId, char[] sessionKey)
		{			
			Console.WriteLine("SessionServer.SendCreateRsp");
						
			this.sessionKey = sessionKey;
			
			UInt32 sessionStatus = 0;
			
			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_CREATE_RSP);
		}
		
		public void SendTermRsp(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionServer.SendTermRsp");

			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_TERM_RSP);
		}
		
		public void SendParticipantsListRsp(ArrayList participants)
		{
			Console.WriteLine("SessionServer.SendParticipantsListRsp");
			
			/*Bytes for storing length*/
			int length = 2;
			
			/*Determine length of buffer*/
			foreach(string username in participants)
			{
				length += username.Length + 2;
			}

			byte[] buffer = new byte[length];
			BinaryWriter s = new BinaryWriter(new MemoryStream(buffer));
	
			s.Write((UInt16) length);
			
			/*Write to buffer*/
			foreach(string username in participants)
			{
				s.Write((UInt16) username.Length);
				s.Write(username.ToCharArray());
			}
						
			Send(buffer, PDU_SESSION_PARTICIPANTS_RSP);
			
		}
		
		public void SendNotificationRsp(string type, string username)
		{
			Console.WriteLine("SessionServer.SendNotificationRsp" + type + " " + username);
			
			int length = 2;
			length += type.Length + 2;
			length += username.Length +2;
			
			byte[] buffer = new byte[length];
			BinaryWriter s = new BinaryWriter(new MemoryStream(buffer));
	
			s.Write((UInt16) length);
			
			s.Write((UInt16) type.Length);
			s.Write(type.ToCharArray());
			
			s.Write((UInt16) username.Length);
			s.Write(username.ToCharArray());
			
						
			Send(buffer, PDU_SESSION_NOTIFICATION_RSP);
		}
		
		private void RecvJoinReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvJoinReq");
			
			UInt32 sessionId;
			char[] sessionKey;
			sessionId = s.ReadUInt32();
			sessionKey = s.ReadChars(12);
					
			listener.OnSessionJoinRequested(sessionKey);
		}
		
		private void RecvLeaveReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvLeaveReq");

			UInt32 sessionId = s.ReadUInt32();

			listener.OnSessionLeaveRequested(sessionId);
		}
		
		private void RecvAuthReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvAuthReq");

			UInt32 sessionId;
			UInt16 usernameLength;
			UInt16 passwordLength;
			string username = "";
			string password = "";
			
			sessionId = s.ReadUInt32();
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
					
			listener.OnSessionAuthenticationRequested(sessionId, username, password);
		}
		
		private void RecvCreateReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvCreateReq");
			
			UInt32 sessionId;
			string username = "";
			string password = "";
			UInt16 usernameLength;
			UInt16 passwordLength;
			
			sessionId = s.ReadUInt32();
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
			
			listener.OnSessionCreateRequested(username, password);
		}
		
		private void RecvTermReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvTermReq");

			UInt32 sessionId;
			char[] sessionKey;
			UInt32 sessionStatus;
						
			sessionId = s.ReadUInt32();
			sessionKey = s.ReadChars(12);
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			listener.OnSessionTerminationRequested(sessionId, sessionKey, sessionStatus);
		}
				
		public override void OnRecv(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionServer.OnRecv");
			
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
		
		public override void OnClose()
		{
			
		}
		
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_SESSION_JOIN_REQ:
					RecvJoinReq(s);
					return;
				
				case PDU_SESSION_LEAVE_REQ:
					RecvLeaveReq(s);
					return;
				
				case PDU_SESSION_CREATE_REQ:
					RecvCreateReq(s);
					return;
				
				case PDU_SESSION_TERM_REQ:
					RecvTermReq(s);
					return;
				
				case PDU_SESSION_AUTH_REQ:
					RecvAuthReq(s);
					return;
				
				default:
					return;
			}
		}
		
		public void ChannelThreadProc()
		{			
			Console.WriteLine("SessionServer.ChannelThreadProc");
			
			while (true)
			{
				lock (channelLock)
				{
					Monitor.Wait(channelLock);
					PDU pdu = (PDU) queue.Dequeue();
					ProcessPDU(pdu.Buffer, pdu.Type);
					Monitor.Pulse(channelLock);
				}
			}
		}
	}
}
