using System;

namespace Screenary
{
	public interface IConnectObserver
	{
		void NewConnection(string address, int port);
	}
}

