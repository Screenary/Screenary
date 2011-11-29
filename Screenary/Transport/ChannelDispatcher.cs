using System;
using System.Threading;
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
		
		public void RegisterChannel(Channel channel)
		{		
			channels.Add(channel.GetChannelId(), channel);
		}
		
		public void OnConnect()
		{
			Channel channel;
			
			foreach (DictionaryEntry entry in channels)
			{
				channel = (Channel) entry.Value;
				channel.OnOpen();
			}
		}
		
		public void OnDisconnect()
		{
			Channel channel;
			
			foreach (DictionaryEntry entry in channels)
			{
				channel = (Channel) entry.Value;
				channel.OnClose();
			}
		}
		
		public void SendPDU(PDU pdu)
		{
			Channel channel = (Channel) channels[pdu.ChannelId];
			channel.Send(pdu.Buffer, pdu.Type);
		}
		
		public void DispatchPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			Channel channel;
			
			channel = (Channel) channels[channelId];
			
			if (channel != null)
				channel.OnRecv(buffer, pduType);
		}
	}
}

