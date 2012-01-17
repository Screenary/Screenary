using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using Screenary;

namespace Screenary.Server
{
	public class Client : SurfaceServer, ISessionRequestListener
	{
		private Session session;
		private SurfaceServer surface;
		private ChannelDispatcher dispatcher;
		/* Linked list implementation of FIFO queue, should replace with real queue... LoL */
		private LinkedList<PDU> pduQ = new LinkedList<PDU>();
		private readonly object lockQ = new object();
		private bool usingQ = false;
		
		/**
		 * Class constructor
		 */ 
		public Client(TransportClient transport) : base(transport)
		{
			this.transport = transport;
			dispatcher = new ChannelDispatcher();
			
			transport.SetChannelDispatcher(dispatcher);
			
			surface = new Surface(this.transport);
			dispatcher.RegisterChannel(surface);
			
			session = new Session(this.transport, this);
			dispatcher.RegisterChannel(session);
			
			dispatcher.OnConnect();
			
			transport.StartThread();
			
			
			//Test participants 
			ArrayList participants = new ArrayList();
			participants.Add("terri");
			participants.Add("dona");
			participants.Add("hai-long");
			participants.Add("marc");
			participants.Add("gina");
			participants.Add("marwan");
			session.SendPartipantsListRsp(participants);
		
		}
		
		/**
		 * Enqueue a PDU
		 */
		public void addPDU(PDU pdu)
		{
			enqueue(pdu);
		}
		
		/**
		 * Thread function continuously dequeues PDU queue and sends to client
		 */ 
		private void ReceiverThreadProc()
		{
			PDU pdu = null;
			
			while (true)
			{
				while (pduQ.Count > 0)
				{
					pdu = dequeue();
					surface.SendSurfaceCommand(pdu.Buffer);
				}
			}
		}
		
		/**
		 * Enqueue a PDU
		 */ 
		private void enqueue(PDU pdu)
		{
			wait();
			pduQ.AddLast(pdu);
			signal();
		}
		
		/**
		 * Dequeue a PDU and return the object
		 */ 
		private PDU dequeue()
		{
			wait();
			PDU pdu = pduQ.First.Value;
			pduQ.RemoveFirst();
			signal();
			return pdu;
		}
		
		/**
		 * Block on lock until it's released
		 */ 
		public void wait()
		{
			lock (lockQ)
			{
				while (usingQ == true) Monitor.Wait(lockQ);
				usingQ = true;
			}
		}
		
		/**
		 * Release lock
		 */ 
		public void signal()
		{
			lock (lockQ)
			{
				usingQ = false;
				Monitor.Pulse(lockQ);
			}
		}
		
		public void OnSessionJoinRequested(char[] sessionKey, string username, string password)
		{
			Console.WriteLine("Client.OnSessionJoinRequested");
			string sessionKeyString = new string(sessionKey);
			
			ScreenSessions scr = ScreenSessions.Instance;
			string userid = scr.joinScreenSession(sessionKeyString, username, password);
			
			//error if userid is -2(session is terminated), -3(session does not exist), -4(authentication failed)
			//send the userid instead of sessionStatus
			session.SendJoinRsp(0, sessionKey,userid, 0, new byte());
				
			Console.WriteLine("SessionKey:{0}" +" username: "+username +" password: "+password, sessionKeyString);		


		}
		
		public void OnSessionLeaveRequested(UInt32 sessionId, string sessionKey, string userid)
		{
			Console.WriteLine("Client.OnSessionLeaveRequested");			
			Console.WriteLine("sessionId: {0}" + " sessionkey "+sessionKey+" userid: "+userid, sessionId);
			
			ScreenSessions scr = ScreenSessions.Instance;
			uint resp = (uint) scr.leaveScreenSession(sessionKey, Convert.ToUInt32(userid));
			
			session.SendLeaveRsp(0, resp);
		}

		public void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password)
		{
			Console.WriteLine("Client.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			session.SendAuthRsp(0, 0);
		}
		
		public void OnSessionCreateRequested(string username, string password)
		{
			Console.WriteLine("Client.OnSessionCreateRequested");
			Console.WriteLine("username:{0} password:{1}", username, password);		
			ScreenSessions scr = ScreenSessions.Instance;
			string key_userid = scr.createScreenSession(password);
			session.SendCreateRsp(0, key_userid.ToCharArray());
		}
		
		public void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Client.OnSessionTerminationRequested");

			string sessionKeyString = new string(sessionKey);
			
			String [] str = sessionKeyString.Split('_');
			sessionKeyString = str[0];
			int screenuserid = Convert.ToInt32(str[1]);
			ScreenSessions scr = ScreenSessions.Instance;
			UInt32 status = scr.terminateScreenSession(sessionKeyString, screenuserid);			
			
			session.SendTermRsp(0, sessionKeyString.ToCharArray(), status);
			
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);

			session.SendTermRsp(0, "ABCDEF123456".ToCharArray(), 0);
		}	
		
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("Client.OnSessionOperationFail");
			Console.WriteLine("errorMessage: {0}", errorMessage);
		}	
		
		public void OnSessionPartipantListUpdated(ArrayList participants)
		{
			Console.WriteLine("Client.OnSessionPartipantsListSuccess");
			session.SendPartipantsListRsp(participants);
		}	
	}
}
