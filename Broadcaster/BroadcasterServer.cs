using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Screenary;

namespace Broadcaster
{
	/**
	 * Broadcaster server, current implementation listens for TCP clients 
	 * and broadcasts contents of data/rfx_sample.pcap within 20 seconds
	 */ 
	public class BroadcasterServer
	{
		/* List of TCP Clients */
		private ArrayList receivers;
		
		/* Server socket */
		private TcpListener tcpListener;
		
		/* Thread listens for connections */
		private Thread listenThread;
		
		/**
		 * Class constructor, instantiate server socket and create thread to 
		 * listen for TCP Clients
		 * 
		 * @param address
		 * @param port
		 */
		public BroadcasterServer(String address, int port)
		{
			receivers = new ArrayList();
			tcpListener = new TcpListener(IPAddress.Parse(address), port);
			listenThread = new Thread(new ThreadStart(listenForClients));
			
			listenThread.Start();
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
		
		/**
		 * Thread function, listen for TCP Clients. When a client is accepted
		 * register it to list of receivers
		 */ 
		private void listenForClients()
		{
			tcpListener.Start();
			
			while (true)
			{
				TcpClient client = tcpListener.AcceptTcpClient();
				Receiver receiver = new Receiver(client);
				receivers.Add(receiver);
				Console.WriteLine("TCP Client accepted");
			}
		}
	}
}

