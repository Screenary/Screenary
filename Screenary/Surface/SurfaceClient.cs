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
		
		public SurfaceClient(ISurfaceClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
		}
		
		private bool RecvSurfaceCommand(BinaryReader s)
		{
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
			
					Monitor.Pulse(channelLock);
				}
			}
		}
	}
}

