using System;

namespace Screenary
{
	public interface IChannel
	{
		UInt16 GetChannelId();
		
		void OnOpen();
		void OnClose();
		
		bool OnRecv(byte[] buffer, byte pduType);
		bool Send(byte[] buffer, byte pduType);
	}
}

