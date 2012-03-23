/**
 * Screenary: Real-Time Collaboration Redefined.
 * Input Channel
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
 * Copyright 2011-2012 Marwan Samaha <mar6@hotmail.com>
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
using System.IO;
using System.Threading;
using System.Collections;

namespace Screenary
{
	public abstract class InputChannel : Channel
	{
		protected Queue queue;
		protected Thread thread;
		protected TransportClient transport;
		
		public const UInt16 PDU_CHANNEL_INPUT = 0x0002;
		
		public const byte PDU_INPUT_KEYBOARD = 0x01;
		public const byte PDU_INPUT_MOUSE = 0x02;		
		
		public InputChannel()
		{
			queue = new Queue();
		}
		
		public override UInt16 GetChannelId()
		{
			return PDU_CHANNEL_INPUT;
		}
		
		public override abstract void OnOpen();
		public override abstract void OnClose();
		
		public override abstract void OnRecv(byte[] buffer, byte pduType);
		
		public override void Send(byte[] buffer, byte pduType)
		{
			transport.SendPDU(buffer, PDU_CHANNEL_INPUT, pduType);
		}
	}
}

