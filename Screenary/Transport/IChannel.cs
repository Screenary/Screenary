using System;

namespace Screenary
{
	public interface IChannel
	{
		UInt16 GetChannelType();
		bool OnRecv(byte[] buffer, byte pduType);
	}
}

