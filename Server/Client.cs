using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Screenary;

namespace Screenary.Server
{
	public class Client : SurfaceServer, ISessionRequestListener, ISurfaceServer
	{
		private Session session;
		private SurfaceServer surface;
		private ChannelDispatcher dispatcher;
		private IClientRequestListener clientReqListener;
		private ISurfaceServer surfaceServerListener;//TA
		
		/* Linked list implementation of FIFO queue, should replace with real queue... LoL */
		private LinkedList<PDU> pduQ = new LinkedList<PDU>();
		private readonly object lockQ = new object();
		private bool usingQ = false;
		
		/**
		 * Class constructor
		 */ 
		public Client(TransportClient transport, IClientRequestListener clientReqListener, ISurfaceServer surfaceServerListener)
			: base(surfaceServerListener, transport)
		{
			this.thread = new Thread(ReceiverThreadProc);
			thread.Start();
			
			this.transport = transport;
			this.clientReqListener = clientReqListener;
			this.surfaceServerListener = surfaceServerListener;//TA
			
			dispatcher = new ChannelDispatcher();
			
			transport.SetChannelDispatcher(dispatcher);
			
			surface = new Surface(this, this.transport);
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
			
			clientReqListener.OnSessionJoinRequested(this, sessionKey, ref sessionId, ref sessionStatus, ref sessionFlags);
			session.SendJoinRsp(sessionId, sessionKey, sessionStatus, sessionFlags);
		}
		
		public void OnSessionLeaveRequested(UInt32 sessionId, string username)
		{
			Console.WriteLine("Client.OnSessionLeaveRequested");
			Console.WriteLine("sessionId: {0}", sessionId);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientReqListener.OnSessionLeaveRequested(this, sessionId, session.sessionKey, ref sessionStatus, username);
			session.SendLeaveRsp(sessionId, sessionStatus);		
		}

		public void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password)
		{
			Console.WriteLine("Client.OnSessionAuthenticationRequested");
			Console.WriteLine("sessionId:{0} username:{1} password:{2}", sessionId, username, password);
			
			UInt32 sessionStatus = UInt32.MaxValue;
			
			clientReqListener.OnSessionAuthenticationRequested(this, sessionId, session.sessionKey, username, password, ref sessionStatus);
			session.SendAuthRsp(sessionId, sessionStatus);
		}
		
		public void OnSessionCreateRequested(string username, string password)
		{
			Console.WriteLine("Client.OnSessionCreateRequested");
			Console.WriteLine("username:{0} password:{1}", username, password);
			
			UInt32 sessionId = UInt32.MaxValue;
			char[] sessionKey = "000000000000".ToCharArray();
			
			clientReqListener.OnSessionCreateRequested(this, username, password, ref sessionId, ref sessionKey);
			session.SendCreateRsp(sessionId, sessionKey);
		}
		
		public void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus)
		{
			Console.WriteLine("Client.OnSessionTerminationRequested");
			string sessionKeyString = new string(sessionKey);
			Console.WriteLine("SessionId:{0}, SessionStatus:{1}, SessionKey:{2}", sessionId, sessionStatus, sessionKeyString);

			clientReqListener.OnSessionTerminationRequested(this, sessionId, sessionKey, ref sessionStatus);
			session.SendTermRsp(sessionId, sessionKey, sessionStatus);
		}	
		
		public void OnSessionOperationFail(string errorMessage)
		{
			Console.WriteLine("Client.OnSessionOperationFail");
			Console.WriteLine("errorMessage: {0}", errorMessage);
		}	
		
		public void OnSessionParticipantListUpdated(ArrayList participants)
		{
			Console.WriteLine("Client.OnSessionParticipantsListUpdated");
			session.SendParticipantsListRsp(participants);
		}	
		
		public void OnSessionNotificationUpdate(string type, string username)
		{
			Console.WriteLine("Client.OnSessionNotificationUpdate");
			session.SendNotificationRsp(type, username);
		}
		
		public void OnSurfaceCommand(char[] sessionKey, byte[] buffer)
		{
			Console.WriteLine("Client.OnSurfaceCommand");
			surfaceServerListener.OnSurfaceCommand(sessionKey, buffer);	
		}
	}
}