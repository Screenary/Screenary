using System;

namespace Screenary
{
	public interface IChannel
	{
		UInt16 GetChannelId();
		
		void OnOpen();
		void OnClose();
		
		void OnRecv(byte[] buffer, byte pduType);
		void Send(byte[] buffer, byte pduType);
	}
}

