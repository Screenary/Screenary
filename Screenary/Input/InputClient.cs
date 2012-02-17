using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class InputClient : InputChannel
	{
		private bool active;
		protected UInt32 sessionId;
		private IInputListener listener;
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
		
		public bool Active { get { return active; } set { active = value; } }
		
		public InputClient(TransportClient transport)
		{
			this.transport = transport;
			this.listener = null;
			this.active = false;
		}
		
		public void SetListener(IInputListener listener)
		{
			this.listener = listener;
		}
		
		public void SetSessionId(UInt32 sessionId)
		{
			this.sessionId = sessionId;	
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		public void SendMouseEvent(uint button, bool down, int x, int y)
		{
			UInt16 pointerFlags;
			byte[] buffer = null;
			int length = sizeof(UInt16) * 3;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
			
			pointerFlags = (down) ? PTR_FLAGS_DOWN : (UInt16) 0;
			
			if (button == 1)
				pointerFlags |= PTR_FLAGS_BTN1;
			else if (button == 2)
				pointerFlags |= PTR_FLAGS_BTN3;
			else if (button == 3)
				pointerFlags |= PTR_FLAGS_BTN2;
			
			s.Write((UInt16) pointerFlags);
			s.Write((UInt16) x);
			s.Write((UInt16) y);

			Send(buffer, PDU_INPUT_MOUSE);
		}
		
		public void SendMouseMotionEvent(int x, int y)
		{
			byte[] buffer = null;
			int length = sizeof(UInt16) * 3;
			BinaryWriter s = InitReqPDU(ref buffer, length, this.sessionId);
		
			s.Write((UInt16) PTR_FLAGS_MOVE);
			s.Write((UInt16) x);
			s.Write((UInt16) y);

			Send(buffer, PDU_INPUT_MOUSE);
		}
		
		public void RecvMouseEvent(BinaryReader s)
		{
			UInt32 sessionId;
			UInt16 pointerFlags;
			UInt16 x, y;
			
			sessionId = s.ReadUInt32();
			pointerFlags = s.ReadUInt16();
			x = s.ReadUInt16();		
			y = s.ReadUInt16();
			
			if (listener != null)
				listener.OnMouseEvent(pointerFlags, x, y);
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
				Console.WriteLine("Received keyboard down event keyCode: {0}", keyCode);
				//listener.OnKeyBoardPressedReceived(x,y);
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

