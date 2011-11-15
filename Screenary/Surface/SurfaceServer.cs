using System;

namespace Screenary
{
	public class SurfaceServer : SurfaceChannel
	{
		ISurfaceServer server;
		
		public SurfaceServer(ISurfaceServer server, TransportClient transport)
		{
			this.server = server;
			this.transport = transport;
		}
		
		public bool SendSurfaceCommand(byte[] buffer)
		{
			return transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, PDU_SURFACE_COMMAND);
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			return true;
		}
	}
}

