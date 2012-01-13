using System;

namespace Screenary.Client
{
	public class Session : SessionClient
	{				
		public Session(TransportClient transport, ISessionResponseListener listener) : base(transport, listener)
		{
		}	
	}
}

