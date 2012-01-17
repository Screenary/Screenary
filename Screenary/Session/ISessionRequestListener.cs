using System;

namespace Screenary
{
	public interface ISessionRequestListener
	{
		void OnSessionJoinRequested(char[] sessionKey, string username, string password);
		void OnSessionLeaveRequested(UInt32 sessionId, string sessionKey, string userid);
		void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password);
		void OnSessionCreateRequested(string username, string password);
		void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus);
		void OnSessionOperationFail(string errorMessage);
	}
}

