using System;

namespace Screenary.Server
{
	public class Surface : SurfaceServer
	{
		public Surface(ISurfaceServer listener, TransportClient transport) : base(listener, transport)
		{

		}
	}
}

