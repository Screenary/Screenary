using System;

namespace Screenary.Client
{
	public interface IUserAction
	{
		void OnUserConnect(string address, int port);
		void OnUserCreateSession(string username, string password);
	}
}

