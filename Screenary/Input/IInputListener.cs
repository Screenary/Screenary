using System;

namespace Screenary
{
	public interface IInputListener
	{
		void OnMouseEvent(UInt16 pointerFlags, UInt16 x, UInt16 y);
		void OnKeyboardEvent(UInt16 keyboardFlags, UInt16 keyCode);
	}
}
