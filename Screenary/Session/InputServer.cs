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
		
		public InputServer (TransportClient transport, ISessionRequestListener listener)
		{
			this.transport = transport;
			this.listener = listener;
		}

					
		private void RecvMouseEvent(BinaryReader s)
		{
			Console.WriteLine("InputServer.RecvMouseEvent");
			
			UInt32 sessionId;
			UInt16 pointerFlag;
			double x, y;
			
			sessionId = s.ReadUInt32();
			pointerFlag = s.ReadUInt16();
			x = s.ReadDouble();			
			y = s.ReadDouble();
		
			Console.WriteLine("Received mouse event "+pointerFlag+": "+x+", "+y+" - for sessionid: "+sessionId);
			
					
			//listener.OnSessionJoinRequested(sessionKey);
		}
		
		public override void OnRecv(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionServer.OnRecv");
			
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
					Monitor.Wait(channelLock);
					PDU pdu = (PDU) queue.Dequeue();
					ProcessPDU(pdu.Buffer, pdu.Type);
					Monitor.Pulse(channelLock);
				}
			}
		}
		
	}
}

