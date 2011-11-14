using System;

namespace Screenary
{
	public class TransportClient
	{
		private Int32 port;
		private string hostname;
		private ChannelDispatcher dispatcher;
		
		public TransportClient(ChannelDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}
		
		public void SetChannelDispatcher(ChannelDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}
		
		public bool SendPDU(byte[] buffer, UInt16 channel, byte pduType)
		{
			return true;
		}
		
		public bool RecvPDU()
		{
			byte[] buffer = null;
			UInt16 channel = 0;
			byte pduType = 0;
			
			
			
			return dispatcher.DispatchPDU(buffer, channel, pduType);
		}
		
		public bool Connect(string hostname, Int32 port)
		{
			this.hostname = hostname;
			this.port = port;
			
			
			
			return true;
		}
		
		public bool Disconnect()
		{
			return true;
		}
	}
}

