using System;

namespace Screenary.Server
{
	public class Session : SessionServer
	{			
		private SESSION_RSP_PDU_HEADER SessionRspPDUHeader;
		
		public Session(TransportClient transport) : base(transport)
		{
			SessionRspPDUHeader.sharedHeader = sharedPDUHeader;
		}
				
		public void SendJoinRsp(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Session.SendJoinRsp");
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_JOIN_RSP;
			
			SESSION_ID session_id;
			session_id.id = sessionId;
			
			SESSION_STATUS session_status;
			session_status.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = session_id;
			SessionRspPDUHeader.sessionStatus = session_status;
			
			SESSION_SHARED_KEY session_shared_key;
			session_shared_key.key = sessionKey;
			
			SESSION_JOIN_RSP_PDU session_join_rsp_pdu;
			session_join_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_join_rsp_pdu.sessionKey = session_shared_key;
			session_join_rsp_pdu.sessionFlags = SESSION_FLAGS_PASSWORD_PROTECTED;

			SendJoinRsp(session_join_rsp_pdu);
		}
				
		public void SendLeaveRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("Session.SendLeaveRsp");
						
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_LEAVE_RSP;
					
			SESSION_ID session_id;
			session_id.id = sessionId;
			
			SESSION_STATUS session_status;
			session_status.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = session_id;
			SessionRspPDUHeader.sessionStatus = session_status;
			
			SESSION_LEAVE_RSP_PDU session_leave_rsp_pdu;
			session_leave_rsp_pdu.sessionHeader = SessionRspPDUHeader;

			SendLeaveRsp(session_leave_rsp_pdu);
		}
		
		public void SendAuthRsp(UInt32 sessionId, UInt32 sessionStatus)
		{
			Console.WriteLine("Session.SendAuthRsp");
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_AUTH_RSP;
			
			SESSION_ID session_id;
			session_id.id = sessionId;
			
			SESSION_STATUS session_status;
			session_status.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = session_id;
			SessionRspPDUHeader.sessionStatus = session_status;
			
			SESSION_AUTH_RSP_PDU session_auth_rsp_pdu;
			session_auth_rsp_pdu.sessionHeader = SessionRspPDUHeader;

			SendAuthRsp(session_auth_rsp_pdu);
		}
				
		public void SendCreateRsp(UInt32 sessionId, char[] sessionKey)
		{ 
			Console.WriteLine("Session.SendCreateRsp");
			
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_CREATE_RSP;
						
			SESSION_ID session_id;
			session_id.id = sessionId;
			
			SessionRspPDUHeader.sessionId = session_id;
			
			SESSION_SHARED_KEY session_shared_key;
			session_shared_key.key = sessionKey;
			
			SESSION_CREATE_RSP_PDU session_create_rsp_pdu;
			session_create_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_create_rsp_pdu.sessionKey = session_shared_key;

			SendCreateRsp(session_create_rsp_pdu);
		}
		
		public void SendTermRsp(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Session.SendTermRsp");
						
			SessionRspPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_RSP;
					
			SESSION_ID session_id;
			session_id.id = sessionId;
			
			SESSION_STATUS session_status;
			session_status.status = sessionStatus;
			
			SessionRspPDUHeader.sessionId = session_id;
			SessionRspPDUHeader.sessionStatus = session_status;
			
			SESSION_SHARED_KEY session_shared_key;
			session_shared_key.key = sessionKey;
			
			sharedPDUHeader.pduType = PDU_SESSION_TERM_RSP;
			
			SESSION_TERM_RSP_PDU session_term_rsp_pdu;
			session_term_rsp_pdu.sessionHeader = SessionRspPDUHeader;
			session_term_rsp_pdu.sessionKey = session_shared_key;

			SendTermRsp(session_term_rsp_pdu);
		}
			
		protected override void RecvJoinReq(SESSION_JOIN_REQ_PDU session_join_req_pdu)
		{
			Console.WriteLine("Session.RecvJoinReq");
			
			char[] sessionKey = session_join_req_pdu.sessionKey.key;		
			
			string sessionKeyString = "";
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}
			Console.WriteLine("SessionKey:{0}", sessionKeyString);
		}
		
		protected override void RecvLeaveReq(SESSION_LEAVE_REQ_PDU session_leave_req_pdu)
		{
			Console.WriteLine("Session.RecvLeaveReq");
			
			UInt32 sessionId = session_leave_req_pdu.sessionHeader.sessionId.id;;
			
			Console.WriteLine("sessionId: {0}", sessionId);			
		}
		
		protected override void RecvAuthReq(SESSION_AUTH_REQ_PDU session_auth_req_pdu)
		{
			Console.WriteLine("Session.RecvAuthReq");
			
			UInt32 sessionId = session_auth_req_pdu.sessionHeader.sessionId.id;
			string username = session_auth_req_pdu.username;
			string password = session_auth_req_pdu.password;
			
			Console.WriteLine("RecvAuthRsp: sessionId:{0} username:{1} password:{2}", 
				sessionId, username, password);
		}
		
		protected override void RecvCreateReq(SESSION_CREATE_REQ_PDU session_create_req_pdu)
		{
			Console.WriteLine("Session.RecvCreateReq");
			
			string username = session_create_req_pdu.username;
			string password = session_create_req_pdu.password;	
			
			Console.WriteLine("username:{0} password:{1}", username, password);
		}
		
		protected override void RecvTermReq(SESSION_TERM_REQ_PDU session_term_req_pdu)
		{
			Console.WriteLine("Session.RecvTermReq");
			
			UInt32 sessionId = session_term_req_pdu.sessionHeader.sessionId.id;
			char[] sessionKey = session_term_req_pdu.sessionKey.key;
			UInt32 sessionStatus = session_term_req_pdu.sessionHeader.sessionStatus.status;
			
			string sessionKeyString = "";
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);
	
		}
		
	}
}

