using System;
using System.IO;

namespace Screenary
{
	public class SurfaceChannel : IChannel
	{
		public const UInt16 PDU_CHANNEL_SURFACE = 0x0001;
		
		public const byte PDU_SURFACE_COMMAND = 0x01;
		
		public SurfaceChannel()
		{
		}
		
		public UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SURFACE;
		}
		
		public virtual bool OnRecv(byte[] buffer, byte pduType)
		{
			return true;
		}
	}
}

