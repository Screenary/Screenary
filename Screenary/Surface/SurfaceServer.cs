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
		
		public bool SendSurfaceCommand(byte[] buffer)
		{
			Console.WriteLine("SendSurfaceCommand");
			return transport.SendPDU(buffer, PDU_CHANNEL_SURFACE, PDU_SURFACE_COMMAND);
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			return true;
		}
	}
}

