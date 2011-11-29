using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public class SessionClient : SessionChannel
	{
		private UInt32 sessionId;
		private ISessionClient client;
		private readonly object channelLock = new object();
		
		public SessionClient(ISessionClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
			this.sessionId = 0;
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}
		
		public void SendJoinReq()
		{

		}
		
		public void SendLeaveReq()
		{

		}
		
		public void SendAuthReq()
		{

		}
		
		public void SendCreateReq(string username, string password)
		{
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
			
			Console.WriteLine("SendCreateReq");
			
			Send(buffer, PDU_SESSION_CREATE_REQ);
		}
		
		public void SendTermReq()
		{

		}
		
		public void RecvJoinRsp(BinaryReader s)
		{

		}
		
		public void RecvLeaveRsp(BinaryReader s)
		{
	
		}
		
		public void RecvAuthRsp(BinaryReader s)
		{
		
		}
		
		public void RecvCreateRsp(BinaryReader s)
		{
			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			sessionKey = s.ReadChars(12);
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKey);
		}
		
		public void RecvTermRsp(BinaryReader s)
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
				case PDU_SESSION_JOIN_RSP:
					RecvJoinRsp(s);
					return;
				
				case PDU_SESSION_LEAVE_RSP:
					RecvLeaveRsp(s);
					return;
				
				case PDU_SESSION_CREATE_RSP:
					RecvCreateRsp(s);
					return;
				
				case PDU_SESSION_TERM_RSP:
					RecvTermRsp(s);
					return;
				
				case PDU_SESSION_AUTH_RSP:
					RecvAuthRsp(s);
					return;
				
				default:
					return;
			}
		}
		
		public void ChannelThreadProc()
		{
			SendCreateReq("screenary", "awesome");
			
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

