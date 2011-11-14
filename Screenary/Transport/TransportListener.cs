using System;

namespace Screenary
{
	public class TransportListener
	{
		private Int32 port;
		private string hostname;
		private ITransportListener listener;
		
		public TransportListener(ITransportListener listener, string hostname, Int32 port)
		{
			this.listener = listener;
			this.hostname = hostname;
			this.port = port;
		}
		
		public bool Start()
		{
			return true;
		}
		
		public bool Stop()
		{
			return true;
		}
	}
}

