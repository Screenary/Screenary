using System;
using System.IO;

namespace Screenary
{
	public class SessionServer : SessionChannel
	{
		private ISessionServer server;
		private TransportClient transport;
		
		public SessionServer(ISessionServer server, TransportClient transport)
		{
			this.server = server;
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
		
		public bool SendJoinRsp()
		{
			return true;
		}
		
		public bool SendLeaveRsp()
		{
			return true;
		}
		
		public bool SendAuthRsp()
		{
			return true;
		}
		
		public bool SendCreateRsp()
		{
			return true;
		}
		
		public bool SendTermRsp()
		{
			return true;
		}
		
		private bool RecvJoinReq(BinaryReader s)
		{
			return true;
		}
		
		private bool RecvLeaveReq(BinaryReader s)
		{
			return true;
		}
		
		private bool RecvAuthReq(BinaryReader s)
		{
			return true;
		}
		
		private bool RecvCreateReq(BinaryReader s)
		{
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
			
			Console.WriteLine("RecvCreateReq: username:{0} password:{1}", username, password);
			
			return true;
		}
		
		private bool RecvTermReq(BinaryReader s)
		{
			return true;
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_SESSION_JOIN_REQ:
					return RecvJoinReq(s);
				
				case PDU_SESSION_LEAVE_REQ:
					return RecvLeaveReq(s);
				
				case PDU_SESSION_CREATE_REQ:
					return RecvCreateReq(s);
				
				case PDU_SESSION_TERM_REQ:
					return RecvTermReq(s);
				
				case PDU_SESSION_AUTH_REQ:
					return RecvAuthReq(s);
				
				default:
					return false;
			}
		}
		
		public override void OnOpen()
		{
		}
		
		public override void OnClose()
		{
		}
	}
}

