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
			Console.WriteLine("SurfaceClient.SendSurfaceCommand");

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
		private bool RecvSurfaceCommand(BinaryReader s)
		{
			Console.WriteLine("SurfaceClient.RecvSurfaceCommand");
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
			while (!stopthread)
			{				
				lock (channelLock)
				{
					while (queue.Count < 1 && !stopthread)
					{
						Monitor.Wait(channelLock);
					}
								
					if(queue.Count >= 1)
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

