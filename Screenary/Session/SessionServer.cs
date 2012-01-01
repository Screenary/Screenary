using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SessionServer : SessionChannel
	{
		private readonly object channelLock = new object();
		
		public SessionServer(TransportClient transport)
		{
			this.transport = transport;
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
		
		public void SendJoinRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionServer.SendJoinRsp");
			
			byte[] buffer = null;
			int length = 4;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			Send(buffer, PDU_SESSION_JOIN_RSP);
		}
		
		public void SendLeaveRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionClient.SendLeaveRsp");
						
			byte[] buffer = null;
			int length = 4;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			Send(buffer, PDU_SESSION_LEAVE_RSP);
		}
		
		public void SendAuthRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionClient.SendAuthRsp");

			byte[] buffer = null;
			int length = + 4;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
									
			Send(buffer, PDU_SESSION_AUTH_RSP);			
		}
		
		public void SendCreateRsp(UInt32 sessionId, char[] sessionKey)
		{
			Console.WriteLine("SessionServer.SendCreateRsp");
			
			byte[] buffer = null;
			int length = sessionKey.Length + 4;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, 0);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_CREATE_RSP);
		}
		
		public void SendTermRsp(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("SessionClient.SendTermRsp");

			byte[] buffer = null;
			int length = sessionKey.Length + 4;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_TERM_RSP);
		}
		
		private void RecvJoinReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvJoinReq");
			
			UInt32 sessionId;
			UInt16 sessionKeyLength;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionKeyLength = s.ReadUInt16();
			string sessionKeyString = "";
			
			if (sessionKeyLength != 12) {
				Console.WriteLine("sessionKeyLength != 12: {0}", sessionKeyLength);
				return;
			}
			
			sessionKey = s.ReadChars(sessionKeyLength);

			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}
			
			Console.WriteLine("SessionKey:{0}", sessionKeyString);
			
		}
		
		private void RecvLeaveReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvLeaveReq");

			UInt32 sessionId = s.ReadUInt32();
			
			Console.WriteLine("sessionId: {0}", sessionId);			
		}
		
		private void RecvAuthReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvAuthReq");

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
			
			Console.WriteLine("RecvAuthRsp: sessionId:{0} username:{1} password:{2}", 
				sessionId, username, password);
		}
		
		private void RecvCreateReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvCreateReq");
			
			UInt32 sessionId;
			string username = "";
			string password = "";
			UInt16 usernameLength;
			UInt16 passwordLength;
			
			sessionId = s.ReadUInt32(); //will read 0
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
			
			Console.WriteLine("username:{0} password:{1}", username, password);
			
			return;
		}
		
		private void RecvTermReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvTermReq");

			UInt32 sessionId;
			UInt16 sessionKeyLength;
			char[] sessionKey;
			UInt32 sessionStatus;
			string sessionKeyString = "";
						
			sessionId = s.ReadUInt32();
			sessionKeyLength = s.ReadUInt16();
			
			if (sessionKeyLength != 12) {
				Console.WriteLine("sessionKeyLength != 12: {0}", sessionKeyLength);
				return;
			}
			
			sessionKey = s.ReadChars(12);

			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}
			
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);
			
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
			Console.WriteLine("SessionServer.OnOpen");
			thread = new Thread(ChannelThreadProc);
			thread.Start();
		}
		
		public override void OnClose()
		{
			Console.WriteLine("SessionServer.OnClose");
			
		}
		
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionServer.ProcessPDU");
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

			//SendJoinRsp(5, 0);
			//SendLeaveRsp(5, 0);
			//SendAuthRsp(5, 0);
			//SendCreateRsp(5, "ABCDEF123456".ToCharArray());
			//SendTermRsp(5, "ABCDEF123456".ToCharArray(), 0);

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

