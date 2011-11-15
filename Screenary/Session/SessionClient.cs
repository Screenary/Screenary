using System;
using System.IO;

namespace Screenary
{
	public class SessionClient : SessionChannel
	{
		private ISessionClient client;
		
		public SessionClient(ISessionClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
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
		
		public bool SendCreateReq()
		{
			return true;
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
	}
}

