using System;

namespace Screenary.Server
{
	public class Session : ISessionServer
	{
		private SessionServer server;
		
		public Session(TransportClient transport)
		{
			server = new SessionServer(this, transport);
		}
		
		public SessionServer GetServer()
		{
			return server;
		}
	}
}

