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
		
		public SurfaceServer(ISurfaceServer listener, TransportClient transport)
		{
			this.listener = listener;
			this.transport = transport;
		}
		
		/*
		 * Send surface commands to client
		 * 
		 * @param pduBuffer
		 * @param sessionKey
		 */
		public void SendSurfaceCommand(byte[] buffer)
		{
			Console.WriteLine("SurfaceServer.SendSurfaceCommand");

			Send(buffer, PDU_SURFACE_COMMAND);
		}
		
		/*
		 * Receive surface updates from client
		 * 
		 * @param s
		 */
		private void RecvSurfaceCommand(BinaryReader s)
		{
			Console.WriteLine("SurfaceServer.RecvSurfaceCommand");
			
			char[] sessionKey = s.ReadChars(12);
			int bufferLength = s.ReadInt16();

			Console.WriteLine("SessionKey: {0}, ByteLength: {1}", new string(sessionKey), bufferLength);

			if (bufferLength > 0) 
			{
				byte[] buffer = s.ReadBytes(bufferLength);
				listener.OnSurfaceCommand(sessionKey, buffer);
			}
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
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			pduType = PDU_SURFACE_COMMAND;
			
			switch (pduType)
			{
				case PDU_SURFACE_COMMAND:
					RecvSurfaceCommand(s);
					return;
				
				default:
					return;
			}

		}
		
		public void ChannelThreadProc()
		{
			while(true)
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

