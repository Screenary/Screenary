/**
 * Screenary: Real-Time Collaboration Redefined.
 * Transport Listener
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
