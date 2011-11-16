using System;
using System.IO;

namespace Screenary
{
	public class SessionClient : SessionChannel
	{
		private UInt32 sessionId;
		private ISessionClient client;
		
		public SessionClient(ISessionClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
			this.sessionId = 0;
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		public bool SendJoinReq()
		{
			return true;
		}
		
		public bool SendLeaveReq()
		{
			return true;
		}
		
		public bool SendAuthReq()
		{
			return true;
		}
		
		public bool SendCreateReq(string username, string password)
		{
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
			
			Console.WriteLine("SendCreateReq");
			
			return Send(buffer, PDU_SESSION_CREATE_REQ);
		}
		
		public bool SendTermReq()
		{
			return true;
		}
		
		public bool RecvJoinRsp(BinaryReader s)
		{
			return true;
		}
		
		public bool RecvLeaveRsp(BinaryReader s)
		{
			return true;
		}
		
		public bool RecvAuthRsp(BinaryReader s)
		{
			return true;
		}
		
		public bool RecvCreateRsp(BinaryReader s)
		{
			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return false;
			}
			
			sessionKey = s.ReadChars(12);
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKey);
			
			return true;
		}
		
		public bool RecvTermRsp(BinaryReader s)
		{
			return true;
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_SESSION_JOIN_RSP:
					return RecvJoinRsp(s);
				
				case PDU_SESSION_LEAVE_RSP:
					return RecvLeaveRsp(s);
				
				case PDU_SESSION_CREATE_RSP:
					return RecvCreateRsp(s);
				
				case PDU_SESSION_TERM_RSP:
					return RecvTermRsp(s);
				
				case PDU_SESSION_AUTH_RSP:
					return RecvAuthRsp(s);
				
				default:
					return false;
			}
		}
		
		public override void OnOpen()
		{
			//SendCreateReq("screenary", "awesome");
		}
		
		public override void OnClose()
		{
		}
	}
}

