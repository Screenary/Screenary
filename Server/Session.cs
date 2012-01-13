using System;

namespace Screenary.Server
{
	public class Session : SessionServer
	{		
		public Session(TransportClient transport, ISessionRequestListener listener) : base(transport, listener)
		{
		}
	}
}

