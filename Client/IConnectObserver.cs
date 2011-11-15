using System;

namespace Screenary.Client
{
	public interface IConnectObserver
	{
		void NewConnection(string address, int port);
	}
}

