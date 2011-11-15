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
		
		public bool SendPDU(PDU pdu)
		{
			IChannel channel = (IChannel) channels[pdu.ChannelId];
			return channel.Send(pdu.Buffer, pdu.Type);
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

