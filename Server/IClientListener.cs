/**
 * Screenary: Real-Time Collaboration Redefined.
 * Client Listener Interface
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

namespace Screenary.Server
{
	public interface IClientListener
	{
		void OnSessionJoinRequested(Client client, char[] sessionKey, ref UInt32 sessionId, ref UInt32 sessionStatus, ref byte sessionFlags);
		void OnSessionLeaveRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, string username);
		void OnSessionAuthenticationRequested(Client client, UInt32 sessionId, char[] sessionKey, string username, string password, ref UInt32 sessionStatus);
		void OnSessionCreateRequested(Client client, string username, string password, ref UInt32 sessionId, ref char[] sessionKey);
		void OnSessionTerminationRequested(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus);
		void OnSessionRemoteAccessRequested(Client client, char[] sessionKey, string username);
		void OnSessionRemoteAccessPermissionSet(Client client, char[] sessionKey, string username, Boolean permission);
		void OnSessionTermRemoteAccessRequested(Client client, char[] sessionKey, string username);
		void OnSurfaceCommand(Client client, UInt32 sessionId, byte[] surfaceCommand);
		void OnRecvMouseEvent(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, UInt16 pointerFlags, int x, int y);			
		void OnRecvKeyboardEvent(Client client, UInt32 sessionId, char[] sessionKey, ref UInt32 sessionStatus, UInt16 pointerFlags, UInt16 keyCode);			
	}
}
