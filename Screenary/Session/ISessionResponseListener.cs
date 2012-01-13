using System;

namespace Screenary
{
	public interface ISessionResponseListener
	{	
		void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected);
		void OnSessionLeaveSuccess();
		void OnSessionAuthenticationSuccess();
		void OnSessionCreationSuccess(char[] sessionKey);
		void OnSessionTerminationSuccess(char[] sessionKey);
		void OnSessionOperationFail(string errorMessage);
	}
}