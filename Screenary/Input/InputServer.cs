/**
 * Screenary: Real-Time Collaboration Redefined.
 * Input Server
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 * Copyright 2011-2012 Marwan Samaha <mar6@hotmail.com>
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
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class InputServer : InputChannel
	{
		private ISessionRequestListener listener;
		private readonly object channelLock = new object();
		
		public InputServer(TransportClient transport, ISessionRequestListener listener)
		{
			this.transport = transport;
			this.listener = listener;
		}
	
		private void RecvMouseEvent(BinaryReader s)
		{
			UInt16 x, y;
			UInt32 sessionId;
			UInt16 pointerFlags;
			
			sessionId = s.ReadUInt32();
			pointerFlags = s.ReadUInt16();
			x = s.ReadUInt16();		
			y = s.ReadUInt16();
			
			if (sessionId != 0)
				listener.OnRecvMouseEvent(sessionId, pointerFlags, x, y);
		}
		
		public void SendMouseEventToSender(UInt16 pointerFlags, int x, int y, UInt32 sessionId)
		{			
			byte[] buffer = null;
			int length = sizeof(UInt16) * 3;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) pointerFlags);
			s.Write((UInt16) x);
			s.Write((UInt16) y);

			Send(buffer, PDU_INPUT_MOUSE);						
		}
		
		public void SendKeyboardEventToSender(UInt16 keyboardFlags, UInt16 keyCode, UInt32 sessionId)
		{
			Console.WriteLine("InputServer.SendKeyboardEventToSender " + sessionId);
			
			byte[] buffer = null;
			int length = sizeof(UInt16) * 2;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) keyboardFlags);
			s.Write((UInt16) keyCode);	
									
			Send(buffer, PDU_INPUT_KEYBOARD);						
		}
		
		private void RecvKeyboardEvent(BinaryReader s)
		{			
			UInt32 sessionId;
			UInt16 keyboardFlags;
			UInt16 keyCode;
			
			sessionId = s.ReadUInt32();
			keyboardFlags = s.ReadUInt16();
			keyCode = s.ReadUInt16();
		
			Console.WriteLine("Received keyboard event {0} - keyCode {1}",
				keyboardFlags, keyCode);
			
			if (sessionId != 0)
				listener.OnRecvKeyboardEvent(sessionId, keyboardFlags, keyCode);
		}
		
		public override void OnRecv(byte[] buffer, byte pduType)
		{			
			lock (channelLock)
			{
				queue.Enqueue(new PDU(buffer, GetChannelId(), pduType));
				Monitor.Pulse(channelLock);
			}
		}
		
		public override void OnOpen()
		{
			thread = new Thread(ChannelThreadProc);
			thread.Start();
		}
		
		public override void OnClose()
		{
			
		}
		
		/**
	 	* Processes a received PDU and calls the appropriate handler
	 	**/
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_INPUT_MOUSE:
					RecvMouseEvent(s);
					return;		
				
				case PDU_INPUT_KEYBOARD:
					RecvKeyboardEvent(s);
					return;
				
				default:
					return;
			}
		}
		
		private BinaryWriter InitRspPDU(ref byte[] buffer, int length, UInt32 id)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) id);
	
			return s;
		}
		
		public void ChannelThreadProc()
		{			
			while (true)
			{
				lock (channelLock)
				{
					Monitor.Wait(channelLock);
					PDU pdu = (PDU) queue.Dequeue();
					ProcessPDU(pdu.Buffer, pdu.Type);
					Monitor.Pulse(channelLock);
				}
			}
		}
		
	}
}
