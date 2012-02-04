using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Screenary;
using System.IO;

namespace Screenary.Server
{
	/**
	 * Broadcaster server, current implementation listens for TCP clients 
	 * and broadcasts contents of data/rfx_sample.pcap within 20 seconds
	 */ 
	public class Broadcaster : ITransportListener, ISurfaceServer
	{		
		
		/* Server socket */
		private TransportListener listener;
		
		/**
		 * Class constructor, instantiate server socket and create thread to 
		 * listen for TCP Clients
		 * 
		 * @param address
		 * @param port
		 */
		public Broadcaster(string address, int port)
		{						
			listener = new TransportListener(this, address, port);
			listener.Start();
		}
		
		public void OnAcceptClient(TransportClient transportClient)
		{
			Console.WriteLine("Broadcaster.OnAcceptClient");
			new Client(transportClient, SessionManager.Instance, this);
		}
		
		/**
		 * Add PDU to clients belonging to a given session
		 * 
		 * @param pdu
		 */
		public void addPDU(PDU pdu, char[] sessionKey)
		{
			SessionManager.Instance.addPDU(pdu, sessionKey);
		}
				
		public void OnSurfaceCommand(char[] sessionKey, byte[] buffer)
		{
			Console.WriteLine("Broadcaster.OnSurfaceCommand");
			PDU pdu = new PDU(buffer, 0, 1);
			this.addPDU(pdu, sessionKey);
		}
		
	}
}

