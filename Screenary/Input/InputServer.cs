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
			//Console.WriteLine("InputServer.RecvMouseEvent");
			
			UInt32 sessionId;
			UInt16 pointerFlag;
			double x, y;
			
			sessionId = s.ReadUInt32();
			pointerFlag = s.ReadUInt16();
			x = s.ReadDouble();			
			y = s.ReadDouble();
		
			Console.WriteLine("Received mouse event {0}: {1}, {2} for sessionId: {3}",
				pointerFlag, x, y, sessionId);
			
			if(sessionId != 0)
				listener.OnRecvMouseEvent(sessionId, pointerFlag, x, y);
		}
		
		public void SendMouseEventToSender(UInt16 pointerFlag, double x, double y, UInt32 sessionId)
		{
			Console.WriteLine("InputServer.SendMouseEventToSender "+sessionId);
			
			byte[] buffer = null;
			int length = sizeof(UInt32) + sizeof(double) * 2;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) pointerFlag);
			s.Write((double) x);
			s.Write((double) y);
									
			Send(buffer, PDU_INPUT_MOUSE);						
		}
		
		public void SendKeyboardEventToSender(UInt16 pointerFlag, UInt16 keyCode, UInt32 sessionId)
		{
			Console.WriteLine("InputServer.SendKeyboardEventToSender "+sessionId);
			
			byte[] buffer = null;
			int length = sizeof(UInt32) + sizeof(UInt16);
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) pointerFlag);
			s.Write((UInt16) keyCode);			
									
			Send(buffer, PDU_INPUT_KEYBOARD);						
		}
		
		private void RecvKeyboardEvent(BinaryReader s)
		{
			//Console.WriteLine("InputServer.RecvKeyboardEvent");
			
			UInt32 sessionId;
			UInt16 pointerFlag;
			UInt16 keyCode;
			
			sessionId = s.ReadUInt32();
			pointerFlag = s.ReadUInt16();
			keyCode = s.ReadUInt16();
		
			Console.WriteLine("Received keyboard event {0} - keyCode {1}",
				pointerFlag, keyCode);
			
			if(sessionId != 0)
				listener.OnRecvKeyboardEvent(sessionId, pointerFlag, keyCode);
		}
		
		
		
		public override void OnRecv(byte[] buffer, byte pduType)
		{
			Console.WriteLine("InputServer.OnRecv");
			
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

