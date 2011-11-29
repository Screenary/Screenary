using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SessionServer : SessionChannel
	{
		private readonly object channelLock = new object();
		
		public SessionServer(TransportClient transport)
		{
			this.transport = transport;
		}
		
		private BinaryWriter InitRspPDU(ref byte[] buffer, int length, UInt32 id, UInt32 status)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 8];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) id);
			s.Write((UInt32) status);
	
			return s;
		}
		
		public void SendJoinRsp()
		{

		}
		
		public void SendLeaveRsp()
		{

		}
		
		public void SendAuthRsp()
		{
			
		}
		
		public void SendCreateRsp()
		{

		}
		
		public void SendTermRsp()
		{

		}
		
		private void RecvJoinReq(BinaryReader s)
		{

		}
		
		private void RecvLeaveReq(BinaryReader s)
		{

		}
		
		private void RecvAuthReq(BinaryReader s)
		{

		}
		
		private void RecvCreateReq(BinaryReader s)
		{
			UInt32 sessionId;
			string username = "";
			string password = "";
			UInt16 usernameLength;
			UInt16 passwordLength;
			
			sessionId = s.ReadUInt32();
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
			
			Console.WriteLine("RecvCreateReq: username:{0} password:{1}", username, password);
			
			return;
		}
		
		private void RecvTermReq(BinaryReader s)
		{

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
			
			switch (pduType)
			{
				case PDU_SESSION_JOIN_REQ:
					RecvJoinReq(s);
					return;
				
				case PDU_SESSION_LEAVE_REQ:
					RecvLeaveReq(s);
					return;
				
				case PDU_SESSION_CREATE_REQ:
					RecvCreateReq(s);
					return;
				
				case PDU_SESSION_TERM_REQ:
					RecvTermReq(s);
					return;
				
				case PDU_SESSION_AUTH_REQ:
					RecvAuthReq(s);
					return;
				
				default:
					return;
			}
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

