using System;
using System.IO;

namespace Screenary
{
	public class SurfaceClient : SurfaceChannel
	{
		private ISurfaceClient client;
		private TransportClient transport;
		
		public SurfaceClient(ISurfaceClient client, TransportClient transport)
		{
			this.client = client;
			this.transport = transport;
		}
		
		private bool RecvSurfaceCommand(BinaryReader s)
		{
			return true;
		}
		
		public override bool OnRecv(byte[] buffer, byte pduType)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader s = new BinaryReader(stream);
			
			switch (pduType)
			{
				case PDU_SURFACE_COMMAND:
					return RecvSurfaceCommand(s);
				
				default:
					return false;
			}
		}
	}
}

