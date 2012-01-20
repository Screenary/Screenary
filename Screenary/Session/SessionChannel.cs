using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SessionChannel : Channel
	{
		protected Queue queue;
		protected Thread thread;
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_SESSION = 0x0000;
		
		public const byte PDU_SESSION_JOIN_REQ = 0x01;
		public const byte PDU_SESSION_LEAVE_REQ = 0x02;
		public const byte PDU_SESSION_CREATE_REQ = 0x03;
		public const byte PDU_SESSION_TERM_REQ = 0x04;
		public const byte PDU_SESSION_AUTH_REQ = 0x05;
		public const byte PDU_SESSION_JOIN_RSP = 0x81;
		public const byte PDU_SESSION_LEAVE_RSP = 0x82;
		public const byte PDU_SESSION_CREATE_RSP = 0x83;
		public const byte PDU_SESSION_TERM_RSP = 0x84;
		public const byte PDU_SESSION_AUTH_RSP = 0x85;
		public const byte PDU_SESSION_PARTICIPANTS_RSP = 0x86;		
		
		public const byte SESSION_FLAGS_PASSWORD_PROTECTED = 0x01;
						
		public SessionChannel()
		{
			queue = new Queue();
		}
		
		public override UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SESSION;
		}
		
		public override abstract void OnOpen();
		public override abstract void OnClose();
		
		public override abstract void OnRecv(byte[] buffer, byte pduType);
		
		public override void Send(byte[] buffer, byte pduType)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_SESSION, pduType);
		}
								
	}
}

