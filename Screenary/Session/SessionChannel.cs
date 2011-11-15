using System;
using System.IO;

namespace Screenary
{
	public abstract class SessionChannel : IChannel
	{
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
		
		public SessionChannel()
		{
		}
		
		public UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SESSION;
		}
		
		public abstract bool OnRecv(byte[] buffer, byte pduType);
		
		public virtual bool Send(byte[] buffer, byte pduType)
		{
			return transport.SendPDU(buffer, PDU_CHANNEL_SESSION, pduType);
		}
	}
}

