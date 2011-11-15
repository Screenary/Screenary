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
	public class BroadcasterServer : ITransportListener
	{
		/* List of TCP Clients */
		private ArrayList receivers;
		
		/* Server socket */
		private TransportListener listener;
		
		/**
		 * Class constructor, instantiate server socket and create thread to 
		 * listen for TCP Clients
		 * 
		 * @param address
		 * @param port
		 */
		public BroadcasterServer(string address, int port)
		{
			receivers = new ArrayList();
			listener = new TransportListener(this, address, port);
			listener.Start();
			
			//while (true)
			{
				TransportClient client;
				
				client = listener.AcceptClient();
				
				Receiver receiver = new Receiver(client);
				receivers.Add(receiver);
				Console.WriteLine("OnAcceptClient");
			}
		}
		
		/**
		 * Add PDU to receivers
		 * 
		 * @param pdu
		 */ 
		public void addPDU(PDU pdu)
		{
			foreach (Receiver receiver in receivers)
			{
				receiver.addPDU(pdu);
			}
		}
		
		public void OnAcceptClient(TransportClient client)
		{

		}
	}
}

