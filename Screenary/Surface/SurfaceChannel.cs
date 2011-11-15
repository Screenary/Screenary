using System;
using System.IO;

namespace Screenary
{
	public abstract class SurfaceChannel : IChannel
	{
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_SURFACE = 0x0001;
		
		public const byte PDU_SURFACE_COMMAND = 0x01;
		
		public SurfaceChannel()
		{
		}
		
		public UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SURFACE;
		}
			
		public abstract void OnOpen();
		public abstract void OnClose();
		
		public abstract bool OnRecv(byte[] buffer, byte pduType);
		
		public virtual bool Send(byte[] buffer, byte pduType)
		{
			return transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, pduType);
		}
	}
}

