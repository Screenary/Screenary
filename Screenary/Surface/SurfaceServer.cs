using System;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SurfaceServer : SurfaceChannel
	{
		ISurfaceServer server;
		private readonly object channelLock = new object();
		
		public SurfaceServer(ISurfaceServer server, TransportClient transport)
		{
			this.server = server;
			this.transport = transport;
		}
		
		public void SendSurfaceCommand(byte[] buffer)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, PDU_SURFACE_COMMAND);
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

		}
		
		public void ChannelThreadProc()
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

