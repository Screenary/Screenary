using System;

namespace Screenary
{
	public interface ITransportListener
	{
		void OnAcceptClient(TransportClient client);
	}
}

