using System;
using System.IO;
using System.Threading;
using System.Collections;


namespace Screenary
{
	public abstract class InputChannel : Channel
	{
		protected Queue queue;
		protected Thread thread;
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_INPUT = 0x0002;
		
		public const byte PDU_INPUT_KEYBOARD = 0x01;
		public const byte PDU_INPUT_MOUSE = 0x02;		
		
		public InputChannel()
		{
			queue = new Queue();
		}
		
		public override UInt16 GetChannelId()
		{
			return PDU_CHANNEL_INPUT;
		}
		
		public override abstract void OnOpen();
		public override abstract void OnClose();
		
		public override abstract void OnRecv(byte[] buffer, byte pduType);
		
		public override void Send(byte[] buffer, byte pduType)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_INPUT, pduType);
		}
	}
}

