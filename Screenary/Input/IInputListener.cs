using System;

namespace Screenary
{
	public interface IInputListener
	{
		void OnMouseEvent(UInt16 pointerFlags, UInt16 x, UInt16 y);
	}
}
