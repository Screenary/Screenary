using System;
using System.Collections;

namespace Screenary
{
	public class ChannelDispatcher
	{
		private Hashtable channels;
		
		public ChannelDispatcher()
		{
			channels = new Hashtable();
		}
		
		public void RegisterChannel(IChannel channel)
		{
			channels.Add(channel, channel.GetChannelId());
		}
		
		public bool DispatchPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			IChannel channel;
			
			channel = (IChannel) channels[channelId];
			channel.OnRecv(buffer, pduType);
			
			return true;
		}
	}
}

