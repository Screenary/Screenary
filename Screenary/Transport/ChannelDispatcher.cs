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
			channels.Add(channel.GetChannelId(), channel);
		}
		
		public bool DispatchPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			IChannel channel;
			
			channelId = 1;
			Console.WriteLine("channelId: {0}", channelId);
			
			channel = (IChannel) channels[channelId];
			channel.OnRecv(buffer, pduType);
			
			return true;
		}
	}
}

