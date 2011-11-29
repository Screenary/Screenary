using System;

namespace Screenary
{
	public abstract class Channel
	{
		public abstract UInt16 GetChannelId();
		
		public abstract void OnOpen();
		public abstract void OnClose();
		
		public abstract void OnRecv(byte[] buffer, byte pduType);
		public abstract void Send(byte[] buffer, byte pduType);
	}
}

