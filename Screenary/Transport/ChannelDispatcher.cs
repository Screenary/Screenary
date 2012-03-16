/**
 * Screenary: Real-Time Collaboration Redefined.
 * Channel Dispatcher
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

