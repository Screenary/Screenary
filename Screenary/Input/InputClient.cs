using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class InputClient : InputChannel
	{
		protected UInt32 sessionId;
		private ISessionResponseListener listener;
		private readonly object channelLock = new object();
		static private bool stopthread = false;
		
		public const UInt16 PTR_FLAGS_MOVE = 0x0800; /* mouse motion */
		public const UInt16 PTR_FLAGS_DOWN = 0x8000; /* button press */
		public const UInt16 PTR_FLAGS_BTN1 = 0x1000; /* button 1 (left) */
		public const UInt16 PTR_FLAGS_BTN2 = 0x2000; /* button 2 (right) */
		public const UInt16 PTR_FLAGS_BTN3 = 0x4000; /* button 3 (middle) */
		
		public const UInt16 KBD_FLAGS_EXTENDED = 0x0100;
		public const UInt16 KBD_FLAGS_DOWN = 0x4000;
		public const UInt16 KBD_FLAGS_RELEASE = 0x8000;
		
		
		public InputClient(ISessionResponseListener listener, TransportClient transport)
		{
			this.transport = transport;
			this.listener = listener;				
		}
		
		public void setSessionId(UInt32 sessionId)
		{
			this.sessionId = sessionId;	
		}
		
		public void sendMouseClick(uint button, double x, double y)
		{
			//Console.WriteLine("InputClient.sendMouseMotion");

			byte[] buffer = null;
			int length = sizeof(UInt32) + sizeof(double) * 2;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
			
			if(button == 1)
				s.Write((UInt16) PTR_FLAGS_BTN1);
			else if(button == 2)
				s.Write((UInt16) PTR_FLAGS_BTN2);
			else if(button == 3)
				s.Write((UInt16) PTR_FLAGS_BTN3);
			else
				s.Write((UInt16) PTR_FLAGS_DOWN);
			
			s.Write((double) x);
			s.Write((double) y);
									
			Send(buffer, PDU_INPUT_MOUSE);
		}
		
		public void sendMouseMotion(double x, double y)
		{
			//Console.WriteLine("InputClient.sendMouseMotion");

			byte[] buffer = null;
			int length = sizeof(UInt32) + sizeof(double) * 2;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
		
			s.Write((UInt16) PTR_FLAGS_MOVE);
			s.Write((double) x);
			s.Write((double) y);
									
			Send(buffer, PDU_INPUT_MOUSE);
		}
		
		public void RecvMouseEvent(BinaryReader s)
		{
			UInt32 sessionId;
			UInt16 pointerFlag;
			double x, y;
			
			sessionId = s.ReadUInt32();
			pointerFlag = s.ReadUInt16();
			x = s.ReadDouble();			
			y = s.ReadDouble();

			if (pointerFlag == PTR_FLAGS_MOVE)
			{
				Console.WriteLine("Received mouse motion: {0}, {1}", x, y);
				//listener.OnMouseMotionReceived(x,y);
			}	
			else
			{
				Console.WriteLine("Received mouse click from button {0}: {1}, {2}", pointerFlag, x, y);
				//listener.OnMouseClickReceived(pointerFlag, x,y);
			}
		}		
		
		public void sendKeyDown(uint keyCode)
		{			

			byte[] buffer = null;
			int length = sizeof(UInt32) + sizeof(UInt16);
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
		
			s.Write((UInt16) KBD_FLAGS_DOWN);
			s.Write((UInt16) keyCode);			
									
			Send(buffer, PDU_INPUT_KEYBOARD);
		}
		
		public void RecvKeyboardEvent(BinaryReader s)
		{
			UInt32 sessionId;
			UInt16 pointerFlag;
			UInt16 keyCode;
			
			sessionId = s.ReadUInt32();
			pointerFlag = s.ReadUInt16();
			keyCode = s.ReadUInt16();						

			if (pointerFlag == KBD_FLAGS_DOWN)
			{
				Console.WriteLine("Received keyboard down event keyCode: {0}",keyCode);
				//listener.OnKeyBoardPressedReceived(x,y);
			}	
			
		}
		
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
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
				Console.WriteLine("closing channel: " + this.ToString());
				Monitor.PulseAll(channelLock);
			}
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
		
		/**
	 	* Code executed by the thread. Listening and processing received packets
	 	**/
		public void ChannelThreadProc()
		{
			Console.WriteLine("InputClient.ChannelThreadProc");
			
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
			
			Console.WriteLine("InputClient.ChannelThreadProc end");
		}
	}
}

