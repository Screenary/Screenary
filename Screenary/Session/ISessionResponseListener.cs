/**
 * Screenary: Real-Time Collaboration Redefined.
 * Session Response Listener Interface 
 *
 * Copyright 2011-2012 Terri-Anne Cambridge <tacambridge@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
		void OnSessionFirstNotificationUpdate(string type, string username, string senderClient);
		void OnSessionRemoteAccessRequestReceived(string username);			
	}
}