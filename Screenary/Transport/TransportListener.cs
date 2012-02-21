using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Screenary
{
	public class TransportListener
	{
		private Int32 port;
		private Thread thread;
		private string address;
		private TcpListener tcpListener;
		private ITransportListener listener;
		
		public TransportListener(ITransportListener listener, string address, Int32 port)
		{
			this.tcpListener = null;
			this.listener = listener;
			this.address = address;
			this.port = port;
		}
		
		public TransportClient AcceptClient()
		{
			TcpClient tcpClient;
			TransportClient client;
				
			tcpClient = tcpListener.AcceptTcpClient();
			tcpClient.NoDelay = true;

			client = new TransportClient(tcpClient);
	
			return client;
		}
		
		public bool Start()
		{
			if (tcpListener == null)
			{
				tcpListener = new TcpListener(IPAddress.Any, 4489);
				thread = new Thread(ListenerThreadProc);
			}
			
			tcpListener.Start();
			thread.Start();
			
			return true;
		}
		
		public bool Stop()
		{
			if (tcpListener != null)
			{
				tcpListener.Stop();
			}
			
			return true;
		}
		
		private void ListenerThreadProc()
		{
			while (true)
			{				
				TransportClient client = AcceptClient();
				listener.OnAcceptClient(client);
			}
		}
	}
}
