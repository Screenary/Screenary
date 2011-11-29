using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Screenary;

namespace Screenary.Server
{
	/**
	 * Broadcaster server, current implementation listens for TCP clients 
	 * and broadcasts contents of data/rfx_sample.pcap within 20 seconds
	 */ 
	public class Broadcaster : ITransportListener
	{
		/* List of TCP Clients */
		private ArrayList clients;
		
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
			clients = new ArrayList();
			listener = new TransportListener(this, address, port);
			listener.Start();
		}
		
		public void OnAcceptClient(TransportClient transportClient)
		{
			Console.WriteLine("OnAcceptClient");
			Client client = new Client(transportClient);
			clients.Add(client);
		}
		
		/**
		 * Add PDU to clients
		 * 
		 * @param pdu
		 */ 
		public void addPDU(PDU pdu)
		{
			foreach (Client client in clients)
			{
				client.addPDU(pdu);
			}
		}
	}
}

