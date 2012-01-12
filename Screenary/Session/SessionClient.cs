using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SessionClient : SessionChannel
	{
		private UInt32 sessionId;
		private readonly object channelLock = new object();
		
		public SessionClient(TransportClient transport)
		{
			this.transport = transport;
			this.sessionId = 0;
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		public void SendJoinReq(char[] sessionKey)
		{
			Console.WriteLine("SessionClient.SendJoinReq");

			byte[] buffer = null;
			int length = sessionKey.Length + 2;
			BinaryWriter s = InitReqPDU(ref buffer, length, 0);
			
			s.Write((UInt16) sessionKey.Length);
			s.Write(sessionKey);
						
			Send(buffer, PDU_SESSION_JOIN_REQ);
		}
		
		public void SendLeaveReq(UInt32 sessionID)
		{
			Console.WriteLine("SessionClient.SendLeaveReq");

			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionID);
									
			Send(buffer, PDU_SESSION_LEAVE_REQ);
		}
		
		public void SendAuthReq(UInt32 sessionID, string username, string password)
		{
			Console.WriteLine("SessionClient.SendAuthReq");

			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionID);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
						
			Send(buffer, PDU_SESSION_AUTH_REQ);
		}
		
		public void SendCreateReq(string username, string password)
		{
			Console.WriteLine("SessionClient.SendCreateReq");

			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, 0);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
				
			Send(buffer, PDU_SESSION_CREATE_REQ);
		}
		
		public void SendTermReq(UInt32 sessionID, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionClient.SendTermReq");
			
			byte[] buffer = null;
			int length = sessionKey.Length + 6;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionID);
			
			s.Write((UInt16) sessionKey.Length);
			s.Write(sessionKey);
			s.Write(sessionStatus);
			
			Send(buffer, PDU_SESSION_TERM_REQ);
		}
		
		public void RecvJoinRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvJoinRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			Console.WriteLine("SessionId: {0}", sessionId);

		}
		
		public void RecvLeaveRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvLeaveRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			Console.WriteLine("SessionId: {0}", sessionId);
		}
		
		public void RecvAuthRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvAuthRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			Console.WriteLine("SessionId: {0}", sessionId);
		}
		
		public void RecvCreateRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvCreateRsp");
			
			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			string sessionKeyString = "";
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			sessionKey = s.ReadChars(12);
			
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}		
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKeyString);
		}
		
		public void RecvTermRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvTermRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			string sessionKeyString = "";
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			sessionKey = s.ReadChars(12);
			
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}		
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKeyString);
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
			Console.WriteLine("SessionClient.OnOpen");
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
				
				default:
					return;
			}
		}
		
		public void ChannelThreadProc()
		{
			Console.WriteLine("SessionClient.ChannelThreadProc");
			
			lock (channelLock)
			{
				while (queue.Count < 1)
				{
					Monitor.Wait(channelLock);
				}
				
				PDU pdu = (PDU) queue.Dequeue();
				ProcessPDU(pdu.Buffer, pdu.Type);
			}
		}
	}
}

