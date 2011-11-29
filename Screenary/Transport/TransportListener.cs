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
		private string hostname;
		private TcpListener tcpListener;
		private ITransportListener listener;
		
		public TransportListener(ITransportListener listener, string hostname, Int32 port)
		{
			this.tcpListener = null;
			this.listener = listener;
			this.hostname = hostname;
			this.port = port;
		}
		
		public TransportClient AcceptClient()
		{
			TcpClient tcpClient;
			TransportClient client;
				
			tcpClient = tcpListener.AcceptTcpClient();
			client = new TransportClient(tcpClient);
	
			return client;
		}
		
		public bool Start()
		{
			if (tcpListener == null)
			{
				IPAddress ipaddr = IPAddress.Parse(hostname);
				tcpListener = new TcpListener(ipaddr, port);
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
