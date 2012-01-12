using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SessionClient : SessionChannel
	{
		protected UInt32 sessionId;
		protected SESSION_RSP_PDU_HEADER SessionRspPDUHeader;
		private readonly object channelLock = new object();
		
		public SessionClient(TransportClient transport)
		{
			this.transport = transport;
			this.SessionRspPDUHeader.sharedHeader = sharedPDUHeader;
			this.sessionId = 0;
		}
		
		private BinaryWriter InitReqPDU(ref byte[] buffer, int length, UInt32 sessionId)
		{
			BinaryWriter s;
			
			buffer = new byte[length + 4];
			s = new BinaryWriter(new MemoryStream(buffer));
			
			s.Write((UInt32) sessionId);
			return s;
		}

		protected void SendJoinReq(SESSION_JOIN_REQ_PDU session_join_req_pdu)
		{
			Console.WriteLine("SessionClient.SendJoinReq");
			
			UInt32 sessionId = session_join_req_pdu.sessionHeader.sessionId.id; // this is 0
			char[] sessionKey = session_join_req_pdu.sessionKey.key;
			
			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write(sessionKey);
						
			Send(buffer, PDU_SESSION_JOIN_REQ);
		}
		
		protected void SendLeaveReq(SESSION_LEAVE_REQ_PDU session_leave_req_pdu)
		{
			Console.WriteLine("SessionClient.SendLeaveReq");
			
			UInt32 sessionId = session_leave_req_pdu.sessionHeader.sessionId.id;
			
			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
									
			Send(buffer, PDU_SESSION_LEAVE_REQ);
		}
		
		protected void SendAuthReq(SESSION_AUTH_REQ_PDU session_auth_req_pdu)
		{
			Console.WriteLine("SessionClient.SendAuthReq");

			UInt32 sessionId = session_auth_req_pdu.sessionHeader.sessionId.id;
			string username = session_auth_req_pdu.username;
			string password = session_auth_req_pdu.password;
						
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
						
			Send(buffer, PDU_SESSION_AUTH_REQ);
		}
		
		protected void SendCreateReq(SESSION_CREATE_REQ_PDU session_create_req_pdu)
		{
			Console.WriteLine("SessionClient.SendCreateReq");

			UInt32 sessionId = session_create_req_pdu.sessionHeader.sessionId.id;//this is 0
			string username = session_create_req_pdu.username;
			string password = session_create_req_pdu.password;
			
			byte[] buffer = null;
			int length = username.Length + password.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write((UInt16) username.Length);
			s.Write((UInt16) password.Length);
			s.Write(username.ToCharArray());
			s.Write(password.ToCharArray());
				
			Send(buffer, PDU_SESSION_CREATE_REQ);
		}
		
		protected void SendTermReq(SESSION_TERM_REQ_PDU session_term_req_pdu)
		{
			Console.WriteLine("SessionClient.SendTermReq");
			
			UInt32 sessionId = session_term_req_pdu.sessionHeader.sessionId.id;
			char[] sessionKey = session_term_req_pdu.sessionKey.key;
			UInt32 sessionStatus = session_term_req_pdu.sessionHeader.sessionStatus.status;
			
			byte[] buffer = null;
			int length = sessionKey.Length + 4;
			BinaryWriter s = InitReqPDU(ref buffer, length, sessionId);
			
			s.Write(sessionKey);
			s.Write(sessionStatus);
			
			Send(buffer, PDU_SESSION_TERM_REQ);
		}
		
		public void RecvJoinRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvJoinRsp");

			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Join Failed: {0}", sessionStatus);
				return;
			}
			
			this.sessionId = s.ReadUInt32();

			sessionKey = s.ReadChars(12);

			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = SESSION_ID;
			SessionRspPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;

			SESSION_JOIN_RSP_PDU session_join_rsp_pdu;
			session_join_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_join_rsp_pdu.sessionKey = SESSION_SHARED_KEY;
			session_join_rsp_pdu.sessionFlags = SESSION_FLAGS_PASSWORD_PROTECTED;
			
			RecvJoinRsp(session_join_rsp_pdu);
		}
		
		public void RecvLeaveRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvLeaveRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Leave Failed: {0}", sessionStatus);
				return;
			}

			if (sessionId != this.sessionId)
			{
				Console.WriteLine("Session Leave Failed: Wrong sessionId {0}", sessionId);
				return;
			}
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = SESSION_ID;
			SessionRspPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_LEAVE_RSP_PDU session_leave_rsp_pdu;
			session_leave_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			
			RecvLeaveRsp(session_leave_rsp_pdu);
		}
		
		public void RecvAuthRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvAuthRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Authentication Failed: {0}", sessionStatus);
				return;
			}
			
			if (sessionId != this.sessionId)
			{
				Console.WriteLine("Session Authentication Failed: Wrong sessionId {0}", sessionId);
				return;
			}
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = SESSION_ID;
			SessionRspPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_AUTH_RSP_PDU session_auth_rsp_pdu;
			session_auth_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			
			RecvAuthRsp(session_auth_rsp_pdu);
		}
		
		public void RecvCreateRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvCreateRsp");
			
			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			this.sessionId = s.ReadUInt32();
			sessionKey = s.ReadChars(12);
							
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = SESSION_ID;
			SessionRspPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_CREATE_RSP_PDU session_create_rsp_pdu;
			session_create_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_create_rsp_pdu.sessionKey = SESSION_SHARED_KEY;
			
			RecvCreateRsp(session_create_rsp_pdu);
		}
		
		public void RecvTermRsp(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvTermRsp");

			UInt32 sessionId;
			UInt32 sessionStatus;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Termination Failed: {0}", sessionStatus);
				return;
			}
			
			if (sessionId != this.sessionId)
			{
				Console.WriteLine("Session Termination Failed: Wrong sessionId {0}", sessionId);
				return;
			}
			
			sessionKey = s.ReadChars(12);
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = SESSION_ID;
			SessionRspPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_TERM_RSP_PDU session_term_rsp_pdu;
			session_term_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_term_rsp_pdu.sessionKey = SESSION_SHARED_KEY;
			
			RecvTermRsp(session_term_rsp_pdu);
		}
		
		public override void OnRecv(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionClient.OnRecv");
			lock (channelLock)
			{
				queue.Enqueue(new PDU(buffer, GetChannelId(), pduType));
				Monitor.Pulse(channelLock);
			}
		}
		
		public override void OnOpen()
		{
			Console.WriteLine("SessionClient.OnOpen");
			thread = new Thread(ChannelThreadProc);
			thread.Start();
		}
		
		public override void OnClose()
		{
			
		}
		
		private void ProcessPDU(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionClient.ProcessPDU");
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
			Console.WriteLine("SessionClient.ChannelThreadProc");
			
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
		
		protected abstract void RecvJoinRsp(SESSION_JOIN_RSP_PDU session_join_rsp_pdu);
		protected abstract void RecvLeaveRsp(SESSION_LEAVE_RSP_PDU session_leave_rsp_pdu);
		protected abstract void RecvAuthRsp(SESSION_AUTH_RSP_PDU session_auth_rsp_pdu);
		protected abstract void RecvCreateRsp(SESSION_CREATE_RSP_PDU session_create_rsp_pdu);
		protected abstract void RecvTermRsp(SESSION_TERM_RSP_PDU session_term_rsp_pdu);
	}
}

