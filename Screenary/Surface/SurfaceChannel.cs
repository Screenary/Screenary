using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SurfaceChannel : IChannel
	{
		protected Queue queue;
		protected Thread thread;
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_SURFACE = 0x0001;
		
		public const byte PDU_SURFACE_COMMAND = 0x01;
		
		public SurfaceChannel()
		{
			queue = new Queue();
		}
		
		public UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SURFACE;
		}
			
		public abstract void OnOpen();
		public abstract void OnClose();
		
		public abstract void OnRecv(byte[] buffer, byte pduType);
		
		public virtual void Send(byte[] buffer, byte pduType)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, pduType);
		}
	}
}

