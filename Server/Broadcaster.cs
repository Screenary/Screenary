using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
	public class Broadcaster : ITransportListener
	{
		/* Server socket */
		private TransportListener listener;
		private ConcurrentDictionary<TransportClient, Client> clients;
		
		/**
		 * Class constructor, instantiate server socket and create thread to 
		 * listen for TCP Clients
		 * 
		 * @param address
		 * @param port
		 */
		public Broadcaster(string address, int port)
		{
			clients = new ConcurrentDictionary<TransportClient, Client>();
			listener = new TransportListener(this, address, port);
			listener.Start();
		}
		
		public void OnAcceptClient(TransportClient transportClient)
		{
			Console.WriteLine("Broadcaster.OnAcceptClient");
			Client client = new Client(transportClient, SessionManager.Instance);
			clients.TryAdd(transportClient, client);
		}
		
		public void MainLoop()
		{
			while (true)
			{
				Thread.Sleep(10);
			}
		}
	}
}

