/**
 * Screenary: Real-Time Collaboration Redefined.
 * Input Listener Interface
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

namespace Screenary
{
	public interface IInputListener
	{
		void OnMouseEvent(UInt16 pointerFlags, UInt16 x, UInt16 y);
		void OnKeyboardEvent(UInt16 keyboardFlags, UInt16 keyCode);
	}
}
