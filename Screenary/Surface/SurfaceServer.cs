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
			Console.WriteLine("SurfaceServer.SendSurfaceCommand");

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
		private void RecvSurfaceCommand(BinaryReader s)
		{
			Console.WriteLine("SurfaceServer.RecvSurfaceCommand");
			
			UInt32 sessionId = s.ReadUInt32();
			byte[] surfaceCommand = s.ReadBytes((int) s.BaseStream.Length);
			
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
				}
			}
		}
	}
}

