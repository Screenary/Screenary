/**
 * Screenary: Real-Time Collaboration Redefined.
 * Surface Client
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
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SurfaceClient : SurfaceChannel
	{
		private ISurfaceClient client;
		private readonly object channelLock = new object();
		static private bool stopthread = false;
		
		public SurfaceClient(ISurfaceClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
		}
		
		private BinaryWriter InitMsgPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		/*
		 * Send surface commands to broadcaster
		 * 
		 * @param pduBuffer
		 * @param sessionKey
		 */
		public void SendSurfaceCommand(byte[] surfaceCommand, UInt32 sessionId)
		{
			byte[] buffer = null;
			int length = surfaceCommand.Length;
			
			BinaryWriter s = InitMsgPDU(ref buffer, length, sessionId);
			
			s.Write(surfaceCommand);
			
			Send(buffer, PDU_SURFACE_COMMAND);
		}
		
		/*
		 * Receive surface updates from broadcaster
		 * 
		 * @param s
		 */
		private bool RecvSurfaceCommand(BinaryReader s, int length)
		{		
			UInt32 sessionId = s.ReadUInt32();
			
			client.OnSurfaceCommand(s);
			
			return true;
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
			lock (channelLock)
			{
				stopthread = true;
				Console.WriteLine("closing channel: "+this.ToString());
				Monitor.PulseAll(channelLock);
			}
		}
		
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			int length = buffer.Length;
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			pduType = PDU_SURFACE_COMMAND;
			
			switch (pduType)
			{
				case PDU_SURFACE_COMMAND:
					RecvSurfaceCommand(s, length);
					return;
				
				default:
					return;
			}
		}
		
		public void ChannelThreadProc()
		{			
			while (!stopthread)
			{				
				lock (channelLock)
				{
					while (queue.Count < 1 && !stopthread)
					{
						Monitor.Wait(channelLock);
					}
								
					if (queue.Count >= 1)
					{
						PDU pdu = (PDU) queue.Dequeue();
						ProcessPDU(pdu.Buffer, pdu.Type);
					}
					
					Monitor.Pulse(channelLock);
				}
			}
		}
	}
}

