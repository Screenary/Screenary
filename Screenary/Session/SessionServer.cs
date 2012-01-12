using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SessionServer : SessionChannel
	{
		private SESSION_REQ_PDU_HEADER SessionReqPDUHeader;
		private readonly object channelLock = new object();
				
		public SessionServer(TransportClient transport)
		{
			this.transport = transport;
			this.SessionReqPDUHeader.sharedHeader = sharedPDUHeader;
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
		
		protected void SendJoinRsp(SESSION_JOIN_RSP_PDU session_join_rsp_pdu)
		{
			Console.WriteLine("SessionServer.SendJoinRsp");
			
			UInt32 sessionId = session_join_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_join_rsp_pdu.sessionHeader.sessionStatus.status;
			char[] sessionKey = session_join_rsp_pdu.sessionKey.key;
			
			//TODO session_join_rsp_pdu.sessionFlags ?
			
			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			s.Write(sessionKey);

			Send(buffer, PDU_SESSION_JOIN_RSP);
		}
		
		protected void SendLeaveRsp(SESSION_LEAVE_RSP_PDU session_leave_rsp_pdu)
		{
			Console.WriteLine("SessionClient.SendLeaveRsp");
						
			UInt32 sessionId = session_leave_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_leave_rsp_pdu.sessionHeader.sessionStatus.status;
			
			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
						
			Send(buffer, PDU_SESSION_LEAVE_RSP);
		}
		
		protected void SendAuthRsp(SESSION_AUTH_RSP_PDU session_auth_rsp_pdu)
		{
			Console.WriteLine("SessionClient.SendAuthRsp");

			UInt32 sessionId = session_auth_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_auth_rsp_pdu.sessionHeader.sessionStatus.status;
			
			byte[] buffer = null;
			int length = 0;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
									
			Send(buffer, PDU_SESSION_AUTH_RSP);			
		}
		
		protected void SendCreateRsp(SESSION_CREATE_RSP_PDU session_create_rsp_pdu)
		{
			Console.WriteLine("SessionServer.SendCreateRsp");
			
			UInt32 sessionId = session_create_rsp_pdu.sessionHeader.sessionId.id;
			char[] sessionKey = session_create_rsp_pdu.sessionKey.key;
			
			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, 0);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_CREATE_RSP);
		}
		
		protected void SendTermRsp(SESSION_TERM_RSP_PDU session_term_rsp_pdu)
		{
			Console.WriteLine("SessionClient.SendTermRsp");

			UInt32 sessionId = session_term_rsp_pdu.sessionHeader.sessionId.id;
			char[] sessionKey = session_term_rsp_pdu.sessionKey.key;
			UInt32 sessionStatus = session_term_rsp_pdu.sessionHeader.sessionStatus.status;

			byte[] buffer = null;
			int length = sessionKey.Length;
			BinaryWriter s = InitRspPDU(ref buffer, length, sessionId, sessionStatus);
			
			s.Write(sessionKey);
			
			Send(buffer, PDU_SESSION_TERM_RSP);
		}
		
		private void RecvJoinReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvJoinReq");
			
			UInt32 sessionId;
			char[] sessionKey;
			
			sessionId = s.ReadUInt32();
			sessionKey = s.ReadChars(12);
					
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_JOIN_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_JOIN_REQ_PDU session_join_req_pdu;
			session_join_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_join_req_pdu.sessionKey = SESSION_SHARED_KEY;
			
			RecvJoinReq(session_join_req_pdu);
		
		}
		
		private void RecvLeaveReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvLeaveReq");

			UInt32 sessionId = s.ReadUInt32();

			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_LEAVE_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_LEAVE_REQ_PDU session_leave_req_pdu;
			session_leave_req_pdu.sessionHeader = SessionReqPDUHeader;
			
			RecvLeaveReq(session_leave_req_pdu);

		}
		
		private void RecvAuthReq(BinaryReader s)
		{
			Console.WriteLine("SessionClient.RecvAuthReq");

			UInt32 sessionId;
			UInt16 usernameLength;
			UInt16 passwordLength;
			string username = "";
			string password = "";
			
			sessionId = s.ReadUInt32();
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
		
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_AUTH_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_AUTH_REQ_PDU session_auth_req_pdu;
			session_auth_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_auth_req_pdu.usernameLength = usernameLength;
			session_auth_req_pdu.passwordLength = passwordLength;
			session_auth_req_pdu.username = username;
			session_auth_req_pdu.password = password;
			
			RecvAuthReq(session_auth_req_pdu);

		}
		
		private void RecvCreateReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvCreateReq");
			
			UInt32 sessionId;
			string username = "";
			string password = "";
			UInt16 usernameLength;
			UInt16 passwordLength;
			
			sessionId = s.ReadUInt32(); /* should be 0 */
			usernameLength = s.ReadUInt16();
			passwordLength = s.ReadUInt16();
			
			if (usernameLength > 0)
				username = new string(s.ReadChars(usernameLength));
			
			if (passwordLength > 0)
				password = new string(s.ReadChars(passwordLength));
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_AUTH_REQ;

			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;

			SessionReqPDUHeader.sessionId = SESSION_ID;

			SESSION_CREATE_REQ_PDU session_create_req_pdu = new SESSION_CREATE_REQ_PDU();
			session_create_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_create_req_pdu.usernameLength = usernameLength;
			session_create_req_pdu.passwordLength = passwordLength;
			session_create_req_pdu.username = username;
			session_create_req_pdu.password = password;		

			RecvCreateReq(session_create_req_pdu);
		}
		
		private void RecvTermReq(BinaryReader s)
		{
			Console.WriteLine("SessionServer.RecvTermReq");

			UInt32 sessionId;
			char[] sessionKey;
			UInt32 sessionStatus;
						
			sessionId = s.ReadUInt32();
			sessionKey = s.ReadChars(12);
			sessionStatus = s.ReadUInt32();
			
			if (sessionStatus != 0)
			{
				Console.WriteLine("Session Creation Failed: {0}", sessionStatus);
				return;
			}
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = sessionStatus;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			SessionReqPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_TERM_REQ_PDU session_term_req_pdu;
			session_term_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_term_req_pdu.sessionKey = SESSION_SHARED_KEY;
			
			RecvTermReq(session_term_req_pdu);
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
			Console.WriteLine("SessionServer.OnOpen");
			thread = new Thread(ChannelThreadProc);
			thread.Start();
		}
		
		public override void OnClose()
		{
			Console.WriteLine("SessionServer.OnClose");
			
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
			Console.WriteLine("SessionServer.ChannelThreadProc");

			//SendJoinRsp(5, 0);
			//SendLeaveRsp(5, 0);
			//SendAuthRsp(5, 0);
			//SendCreateRsp(5, "ABCDEF123456".ToCharArray());
			//SendTermRsp(5, "ABCDEF123456".ToCharArray(), 0);
			
			while (true)
			{
				lock (channelLock)
				{
					Monitor.Wait(channelLock);
					PDU pdu = (PDU) queue.Dequeue();
					ProcessPDU(pdu.Buffer, pdu.Type);
				}
			}
		}
		
		protected abstract void RecvJoinReq(SESSION_JOIN_REQ_PDU session_join_req_pdu);
		protected abstract void RecvLeaveReq(SESSION_LEAVE_REQ_PDU session_leave_req_pdu);
		protected abstract void RecvAuthReq(SESSION_AUTH_REQ_PDU session_auth_req_pdu);
		protected abstract void RecvCreateReq(SESSION_CREATE_REQ_PDU session_create_req_pdu);
		protected abstract void RecvTermReq(SESSION_TERM_REQ_PDU session_term_req_pdu);
	}
}

