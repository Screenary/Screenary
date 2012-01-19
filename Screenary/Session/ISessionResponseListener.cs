using System;
using System.Collections;

namespace Screenary
{
	public interface ISessionResponseListener
	{	
		void OnSessionJoinSuccess(char[] sessionKey, Boolean isPasswordProtected, string userid);
		void OnSessionLeaveSuccess();
		void OnSessionAuthenticationSuccess();
		void OnSessionCreationSuccess(char[] sessionKey);
		void OnSessionTerminationSuccess(char[] sessionKey);
		void OnSessionOperationFail(string errorMessage);
		void OnSessionParticipantListUpdate(ArrayList participants);
	}
}