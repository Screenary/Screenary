using System;
using System.Collections;

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
		void OnSessionParticipantListUpdate(ArrayList participants);
		void OnSessionNotificationUpdate(string type, string username);
		void OnSessionRemoteAccessRequestReceived(string username);			
	}
}