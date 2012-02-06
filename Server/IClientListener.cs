using System;

namespace Screenary.Server
{
	public interface IClientListener
	{
		void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags);
		void OnSessionLeaveRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, string username);
		void OnSessionAuthenticationRequested(Client client, UInt32 sessionId, char[] sessionKey, string username, string password, ref UInt32 sessionStatus);
		void OnSessionCreateRequested(Client client, string username, string password, ref UInt32 sessionId, ref char[] sessionKey);
		void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus);
		void OnSessionOperationFail(string errorMessage);
		void OnSurfaceCommand(Client client, char[] sessionKey, byte[] surfaceCommand);
	}
}
