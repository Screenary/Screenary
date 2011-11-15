using System;

namespace Screenary.Client
{
	public class Session : ISessionClient
	{
		private SessionClient client;
		
		public Session(TransportClient transport)
		{
			client = new SessionClient(this, transport);	
		}
		
		public SessionClient GetClient()
		{
			return client;
		}
	}
}

