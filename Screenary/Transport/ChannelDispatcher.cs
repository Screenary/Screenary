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
		
		public void OnConnect()
		{
			IChannel channel;
			
			foreach (DictionaryEntry entry in channels)
			{
				channel = (IChannel) entry.Value;
				channel.OnOpen();
			}
		}
		
		public void OnDisconnect()
		{
			IChannel channel;
			
			foreach (DictionaryEntry entry in channels)
			{
				channel = (IChannel) entry.Value;
				channel.OnClose();
			}
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
			
			if (channel != null)
				channel.OnRecv(buffer, pduType);
			
			return true;
		}
	}
}

