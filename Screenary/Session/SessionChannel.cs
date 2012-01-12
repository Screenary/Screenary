using System;
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class SessionChannel : Channel
	{
		protected Queue queue;
		protected Thread thread;
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_SESSION = 0x0000;
		
		public const byte PDU_SESSION_JOIN_REQ = 0x01;
		public const byte PDU_SESSION_LEAVE_REQ = 0x02;
		public const byte PDU_SESSION_CREATE_REQ = 0x03;
		public const byte PDU_SESSION_TERM_REQ = 0x04;
		public const byte PDU_SESSION_AUTH_REQ = 0x05;
		public const byte PDU_SESSION_JOIN_RSP = 0x81;
		public const byte PDU_SESSION_LEAVE_RSP = 0x82;
		public const byte PDU_SESSION_CREATE_RSP = 0x83;
		public const byte PDU_SESSION_TERM_RSP = 0x84;
		public const byte PDU_SESSION_AUTH_RSP = 0x85;
		
		public const byte SESSION_FLAGS_PASSWORD_PROTECTED = 0x01;
		
		protected SHARED_PDU_HEADER sharedPDUHeader;
				
		public SessionChannel()
		{
			queue = new Queue();
			sharedPDUHeader.channel = PDU_CHANNEL_SESSION;
		}
		
		public override UInt16 GetChannelId()
		{
			return PDU_CHANNEL_SESSION;
		}
		
		public override abstract void OnOpen();
		public override abstract void OnClose();
		
		public override abstract void OnRecv(byte[] buffer, byte pduType);
		
		public override void Send(byte[] buffer, byte pduType)
		{
			Console.WriteLine("SessionChannel.Send");
			transport.SendPDU(buffer, PDU_CHANNEL_SESSION, pduType);
		}
		
		protected struct SHARED_PDU_HEADER {
			public UInt16 channel;
			public byte pduType;
			public byte fragFlags;
			public UInt16 fragSize;
		};
						
		/**
		 * The Session Shared Key is a 12-character string consisting only of 
		 * uppercase alphanumeric characters. It is a non null-terminated ASCII 
		 * string of 12 characters, with a fixed size of 12 bytes.
		 */
		protected struct SESSION_SHARED_KEY 
		{ 
			public char[] key; 
		};
				
		/**
		 * The Session ID is an unsigned integer of 32 bits. It uniquely identifies 
		 * a client session. One client may have multiple Session IDs if it participates 
		 * in more than one screencasting session at the same time.
		 */
		protected struct SESSION_ID { 
			public UInt32 id; 
		};
					
		/**
		 * The Session Status is an unsigned integer of 32 bits indicating the status 
		 * of a request. 0 indicates success, while non-zero values indicate an error.
		 */
		protected struct SESSION_STATUS { 
			public UInt32 status; 
		};		
				
		/**
		 * The Session Request PDU Header (10 bytes) is shared by all Session Request PDUs.
		 */
		protected struct SESSION_REQ_PDU_HEADER {
			public SHARED_PDU_HEADER sharedHeader;
			public SESSION_ID sessionId;
			public SESSION_STATUS sessionStatus; //TODO TA added this because of SendTermReq (maybe not needed)
		};
			
		/**
		 * The Session Response PDU Header (14 bytes) is shared by all Session Response PDUs.
		 */
		protected struct SESSION_RSP_PDU_HEADER {
			public SHARED_PDU_HEADER sharedHeader;
			public SESSION_ID sessionId;
			public SESSION_STATUS sessionStatus;
		};
					
		/**
		 * The Session Join Request PDU is sent by the client to request to join an existing 
		 * session using a Session Shared Key.
		 */
		protected struct SESSION_JOIN_REQ_PDU {
			public SESSION_REQ_PDU_HEADER sessionHeader;
			public SESSION_SHARED_KEY sessionKey;
		};
				
		/**
		 * The Session Join Response PDU is sent from server to client in response to a 
		 * Session Join Request.
		 */
		protected struct SESSION_JOIN_RSP_PDU {
			public SESSION_RSP_PDU_HEADER sessionHeader;
			public SESSION_SHARED_KEY sessionKey;
			public byte sessionFlags;
		};
				
		/**
		 * The Session Leave Request PDU is sent by the client to request to leave a 
		 * previously joined session.
		 */
		protected struct SESSION_LEAVE_REQ_PDU {
			public SESSION_REQ_PDU_HEADER sessionHeader;
		};
				
		/**
		 * The Session Leave Response PDU is sent from server to client in response to 
		 * a Session Leave Request PDU.
		 */
		protected struct SESSION_LEAVE_RSP_PDU {
			public SESSION_RSP_PDU_HEADER sessionHeader;
		};
				
		/**
		 * The Session Create Request PDU is sent by the client (session initiator) to 
		 * request a new screencasting session to be created on the server.
		 */
		protected struct SESSION_CREATE_REQ_PDU {
			public SESSION_REQ_PDU_HEADER sessionHeader;
			public UInt16 usernameLength;
			public UInt16 passwordLength;
			//public char* username; TODO
			public string username;
			//public char* password; TODO
			public string password;
		};

		/**
		 * The Session Create Response PDU is sent by the server in response to a 
		 * Session Create Request PDU, and provides the Session Shared Key to the Session 
		 * Initiator.
		 */
		protected struct SESSION_CREATE_RSP_PDU {
			public SESSION_RSP_PDU_HEADER sessionHeader;
			public SESSION_SHARED_KEY sessionKey;
		};		
		
		/**
		 * The Session Termination Request PDU is sent by the client (session controller) 
		 * to request that a screencasting session be terminated, disconnecting all session 
		 * participants.
		 */
		protected struct SESSION_TERM_REQ_PDU {
			public SESSION_REQ_PDU_HEADER sessionHeader;
			public SESSION_SHARED_KEY sessionKey;
		};
			
		/**
		 * The Session Termination Response PDU is sent the server to the requesting 
		 * client only in the case of failure, otherwise it is sent to all session 
		 * participants as a notification of session termination.
		 */
		protected struct SESSION_TERM_RSP_PDU {
			public SESSION_RSP_PDU_HEADER sessionHeader;
			public SESSION_SHARED_KEY sessionKey;
		};
		
		/**
		 * The Session Authenticate Request PDU is sent by the client following the 
		 * reception of a successful Session Join Response.
		 */
		protected struct SESSION_AUTH_REQ_PDU {
			public SESSION_REQ_PDU_HEADER sessionHeader;
			public UInt16 usernameLength;
			public UInt16 passwordLength;
			//public char* username; TODO
			public string username;
			//public char* password; TODO
			public string password;
		};
		
		/**
		 * The Session Authenticate Response PDU is sent by the server in response 
		 * to a Session Authenticate Request PDU. If successful, the client completed 
		 * the authentication sequence, otherwise a status code describing the error 
		 * is provided.
		 */
		protected struct SESSION_AUTH_RSP_PDU {
			public SESSION_RSP_PDU_HEADER sessionHeader;
		};
				
	}
}

