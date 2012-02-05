using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SurfaceChannel : Channel
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
		
		public override UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SURFACE;
		}
			
		public override abstract void OnOpen();
		public override abstract void OnClose();
		
		public override abstract void OnRecv(byte[] buffer, byte pduType);
		
		public override void Send(byte[] buffer, byte pduType)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, pduType);
		}
	}
}

