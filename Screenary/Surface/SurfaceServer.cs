/**
 * Screenary: Real-Time Collaboration Redefined.
 * Surface Server
 *
 * Copyright 2011-2012 Terri-Anne Cambridge <tacambridge@gmail.com>
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
using System.IO;

namespace Screenary
{
	public class SurfaceServer : SurfaceChannel
	{
		private readonly object channelLock = new object();
		private ISurfaceServer listener;
		
		public SurfaceServer(TransportClient transport, ISurfaceServer listener)
		{
			this.transport = transport;
			this.listener = listener;
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
		 * Send surface commands to client
		 * 
		 * @param surfaceCommand
		 * @param sessionId
		 */		
		public void SendSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand)
		{
			byte[] buffer = null;
			int length = surfaceCommand.Length;
			
			BinaryWriter s = InitMsgPDU(ref buffer, length, sessionId);
			
			s.Write(surfaceCommand);
			
			Send(buffer, PDU_SURFACE_COMMAND);
		}
		
		/*
		 * Receive surface updates from client
		 * 
		 * @param s
		 */
		private void RecvSurfaceCommand(BinaryReader s, int length)
		{			
			UInt32 sessionId = s.ReadUInt32();
			byte[] surfaceCommand = s.ReadBytes(length - 4);
			
			listener.OnSurfaceCommand(sessionId, surfaceCommand);
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
			while (true)
			{
				lock (channelLock)
				{
					while (queue.Count < 1)
					{
						Monitor.Wait(channelLock);
					}
					
					PDU pdu = (PDU) queue.Dequeue();
					ProcessPDU(pdu.Buffer, pdu.Type);
				}
			}
		}
	}
}

