/**
 * Screenary: Real-Time Collaboration Redefined.
 * Session Request Listener Interface 
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

namespace Screenary
{
	public interface ISessionRequestListener
	{
		void OnSessionJoinRequested(char[] sessionKey);
		void OnSessionLeaveRequested(UInt32 sessionId, string username);
		void OnSessionAuthenticationRequested(UInt32 sessionId, string username, string password);
		void OnSessionCreateRequested(string username, string password);
		void OnSessionTerminationRequested(UInt32 sessionId, char[] sessionKey, UInt32 sessionStatus);
		void OnSessionRemoteAccessRequested(char[] sessionKey, string username);
		void OnSessionRemoteAccessPermissionSet(char[] sessionKey, string username, Boolean permission);
		void OnSessionTermRemoteAccessRequested(char[] sessionKey, string username);
		void OnRecvMouseEvent(UInt32 sessionId, UInt16 pointerFlags, int x, int y);
		void OnRecvKeyboardEvent(UInt32 sessionId, UInt16 pointerFlags, UInt16 keyCode);
	}
}
