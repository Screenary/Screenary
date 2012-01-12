using System;

namespace Screenary.Client
{
	public class Session : SessionClient
	{		

		private ISessionResponseListener listener;
		private SESSION_REQ_PDU_HEADER SessionReqPDUHeader;
		
		public Session(TransportClient transport, ISessionResponseListener listener) : base(transport)
		{
			SessionReqPDUHeader.sharedHeader = sharedPDUHeader;
			this.listener = listener;
		}

		public void SendJoinReq(char[] sessionKey)
		{
			Console.WriteLine("Session.SendJoinReq");
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_JOIN_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = 0;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_JOIN_REQ_PDU session_join_req_pdu;
			session_join_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_join_req_pdu.sessionKey = SESSION_SHARED_KEY;
			
			SendJoinReq(session_join_req_pdu);
		}
		
		public void SendLeaveReq()
		{
			Console.WriteLine("Session.SendLeaveReq");
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_LEAVE_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = this.sessionId;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_LEAVE_REQ_PDU session_leave_req_pdu;
			session_leave_req_pdu.sessionHeader = SessionReqPDUHeader;

			SendLeaveReq(session_leave_req_pdu);
		}
		
		public void SendAuthReq(string username, string password)
		{
			Console.WriteLine("Session.SendAuthReq");
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_AUTH_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = this.sessionId;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_AUTH_REQ_PDU session_auth_req_pdu;
			session_auth_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_auth_req_pdu.usernameLength = (UInt16) username.Length;
			session_auth_req_pdu.passwordLength = (UInt16) password.Length;
			session_auth_req_pdu.username = username;
			session_auth_req_pdu.password = password;							

			SendAuthReq(session_auth_req_pdu);
		}
		
		public void SendCreateReq(string username, string password)
		{
			Console.WriteLine("Session.SendCreateReq");
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_CREATE_REQ;
						
			SESSION_ID SESSION_ID;
			SESSION_ID.id = 0;
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			
			SESSION_CREATE_REQ_PDU session_create_req_pdu;
			session_create_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_create_req_pdu.usernameLength = (UInt16) username.Length;
			session_create_req_pdu.passwordLength = (UInt16) password.Length;
			session_create_req_pdu.username = username;
			session_create_req_pdu.password = password;				

			SendCreateReq(session_create_req_pdu);
		}
		
		public void SendTermReq(char[] sessionKey)
		{
			Console.WriteLine("Session.SendTermReq");
			
			SessionReqPDUHeader.sharedHeader.pduType = PDU_SESSION_TERM_REQ;
			
			SESSION_ID SESSION_ID;
			SESSION_ID.id = this.sessionId;
			
			SESSION_STATUS SESSION_STATUS;
			SESSION_STATUS.status = 0; //TODO: this is good?
			
			SessionReqPDUHeader.sessionId = SESSION_ID;
			SessionReqPDUHeader.sessionStatus = SESSION_STATUS;
			
			SESSION_SHARED_KEY SESSION_SHARED_KEY;
			SESSION_SHARED_KEY.key = sessionKey;
			
			SESSION_TERM_REQ_PDU session_term_req_pdu;
			session_term_req_pdu.sessionHeader = SessionReqPDUHeader;
			session_term_req_pdu.sessionKey = SESSION_SHARED_KEY;

			SendTermReq(session_term_req_pdu);
		}
		
		protected override void RecvJoinRsp(SESSION_JOIN_RSP_PDU session_join_rsp_pdu)
		{
			UInt32 sessionId = session_join_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_join_rsp_pdu.sessionHeader.sessionStatus.status;
			char[] sessionKey = session_join_rsp_pdu.sessionKey.key;

			string sessionKeyString = "";
	
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}		
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKeyString);
				
			listener.OnRecvJoinRsp(sessionKey);
		}
		
		protected override void RecvLeaveRsp(SESSION_LEAVE_RSP_PDU session_leave_rsp_pdu)
		{
			UInt32 sessionId = session_leave_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_leave_rsp_pdu.sessionHeader.sessionStatus.status;
	
			Console.WriteLine("SessionId: {0}", sessionId);
			
			listener.OnRecvLeaveRsp();
		}

		protected override void RecvAuthRsp(SESSION_AUTH_RSP_PDU session_auth_rsp_pdu)
		{
			UInt32 sessionId = session_auth_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_auth_rsp_pdu.sessionHeader.sessionStatus.status;

			Console.WriteLine("SessionId: {0}", sessionId);

			listener.OnRecvAuthRsp();
		}

		protected override void RecvCreateRsp(SESSION_CREATE_RSP_PDU session_create_rsp_pdu)
		{
			UInt32 sessionId = session_create_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_create_rsp_pdu.sessionHeader.sessionStatus.status;
			char[] sessionKey = session_create_rsp_pdu.sessionKey.key;
			
			string sessionKeyString = "";
	
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}		
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKeyString);
			
			listener.OnRecvCreateRsp(sessionKey);
		}

		protected override void RecvTermRsp(SESSION_TERM_RSP_PDU session_term_rsp_pdu)
		{
			UInt32 sessionId = session_term_rsp_pdu.sessionHeader.sessionId.id;
			UInt32 sessionStatus = session_term_rsp_pdu.sessionHeader.sessionStatus.status;
			char[] sessionKey = session_term_rsp_pdu.sessionKey.key;

			string sessionKeyString = "";
	
			for(int i = 0; i < sessionKey.Length; i++) {
				sessionKeyString += sessionKey[i];
			}		
			
			Console.WriteLine("SessionId: {0}, SessionKey:{1}", sessionId, sessionKeyString);
			
			listener.OnRecvTermRsp(sessionKey);
		}

		
	}
}

