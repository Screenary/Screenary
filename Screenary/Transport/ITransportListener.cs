using System;

namespace Screenary
{
	public interface ITransportListener
	{
		bool OnClientConnect(TransportClient client);
	}
}

