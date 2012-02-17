using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe class Keyboard
	{
		private UInt32 keyboardLayoutId;
		
		[DllImport("libfreerdp-kbd")]
		public static extern UInt32 freerdp_kbd_init(IntPtr display, UInt32 keyboardLayoutId);
		
		[DllImport("libfreerdp-kbd")]
		public static extern byte freerdp_kbd_get_scancode_by_keycode(byte keycode, ref int extended);
		
		[DllImport("libfreerdp-kbd")]
		public static extern byte freerdp_kbd_get_keycode_by_scancode(byte scancode, int extended);
		
		[DllImport("libfreerdp-kbd")]
		public static extern byte freerdp_kbd_get_scancode_by_virtualkey(int vkcode, ref int extended);
		
		public Keyboard()
		{
			keyboardLayoutId = freerdp_kbd_init(IntPtr.Zero, 0);
		}
		
		public int GetScancodeFromKeycode(int keycode)
		{
			int scancode;
			int extended = 0;
			
			scancode = freerdp_kbd_get_scancode_by_keycode((byte) keycode, ref extended);
			
			return scancode;
		}
		
		public int GetKeycodeFromScancode(int scancode)
		{
			int keycode;
			int extended = 0;
			
			keycode = freerdp_kbd_get_scancode_by_keycode((byte) scancode, ref extended);
			
			return keycode;
		}
		
		public int GetScancodeFromVirtualKeyCode(int vkcode)
		{
			int scancode;
			int extended = 0;
			
			scancode = freerdp_kbd_get_scancode_by_virtualkey(vkcode, ref extended);
			
			return scancode;
		}
		
		~Keyboard()
		{

		}
	}
}
