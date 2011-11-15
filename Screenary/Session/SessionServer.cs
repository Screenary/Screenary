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
	}
}

