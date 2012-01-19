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
		private IClientRequestListener listener;
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
		public Client(TransportClient transport, IClientRequestListener listener) : base(transport)
		{
			this.thread = new Thread(ReceiverThreadProc);
			thread.Start();
			
			this.listener = listener;
			this.transport = transport;
			dispatcher = new ChannelDispatcher();
			
			transport.SetChannelDispatcher(dispatcher);
			
			surface = new Surface(this.transport);
			dispatcher.RegisterChannel(surface);
			
			session = new Session(this.transport, this);
			dispatcher.RegisterChannel(session);
			
			dispatcher.OnConnect();
			
			transport.StartThread();
		
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
		
		public void OnSessionJoinRequested(char[] sessionKey)
		{
			Console.WriteLine("Client.OnSessionJoinRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionKey:{0}", sessionKeyString);
			
			UInt32 sessionId = UInt32.MaxValue;
			UInt32 sessionStatus = UInt32.MaxValue;
			byte sessionFlags = 0x00;
			
			listener.OnSessionJoinRequested(this, sessionKey, ref sessionId, ref sessionStatus, ref sessionFlags);
			session.SendJoinRsp(sessionId, sessionKey, sessionStatus, sessionFlags);
		}
		
		public void OnSessionLeaveRequested(UInt32 sessionId)
		{
			Console.WriteLine("Client.OnSessionLeaveRequested");
			Console.WriteLine("sessionId: {0}", sessionId);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			listener.OnSessionLeaveRequested(this, sessionId, session.sessionKey, ref sessionStatus);
			session.SendLeaveRsp(sessionId, sessionStatus);		
		}

		public void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password)
		{
			Console.WriteLine("Client.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			listener.OnSessionAuthenticationRequested(this, sessionId, session.sessionKey, username, password, ref sessionStatus);
			session.SendAuthRsp(sessionId, sessionStatus);
		}
		
		public void OnSessionCreateRequested(string username, string password)
		{
			Console.WriteLine("Client.OnSessionCreateRequested");
			Console.WriteLine("username:{0} password:{1}", username, password);
			
			UInt32 sessionId = UInt32.MaxValue;
			char[] sessionKey = "000000000000".ToCharArray();
			
			listener.OnSessionCreateRequested(this, username, password, ref sessionId, ref sessionKey);
			session.SendCreateRsp(sessionId, sessionKey);
		}
		
		public void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Client.OnSessionTerminationRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);

			listener.OnSessionTerminationRequested(this, sessionId, sessionKey, ref sessionStatus);
			session.SendTermRsp(sessionId, sessionKey, sessionStatus);
		}	
		
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("Client.OnSessionOperationFail");
			Console.WriteLine("errorMessage: {0}", errorMessage);
		}	
		
		public void OnSessionParticipantListUpdated(ArrayList participants)
		{
			Console.WriteLine("Client.OnSessionPartipantsListSuccess");
			session.SendPartipantsListRsp(participants);
		}	
	}
}
