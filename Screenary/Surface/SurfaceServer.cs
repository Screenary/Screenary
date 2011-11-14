using System;

namespace Screenary
{
	public class SurfaceServer : SurfaceChannel
	{
		ISurfaceServer server;
		TransportClient transport;
		
		public SurfaceServer(ISurfaceServer server, TransportClient transport)
		{
			this.server = server;
			this.transport = transport;
		}
		
		public bool SendSurfaceCommand()
		{
			return true;
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			return true;
		}
	}
}

