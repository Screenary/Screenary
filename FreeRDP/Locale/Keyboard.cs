using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe class Keyboard
	{
		private UInt32 keyboardLayoutId;
		
		[DllImport("libfreerdp-locale")]
		public static extern UInt32 freerdp_keyboard_init(UInt32 keyboardLayoutId);
		
		[DllImport("libfreerdp-locale")]
		public static extern UInt32 freerdp_keyboard_get_rdp_scancode_from_x11_keycode(UInt32 keycode, ref int extended);
		
		[DllImport("libfreerdp-locale")]
		public static extern UInt32 freerdp_keyboard_get_x11_keycode_from_rdp_scancode(UInt32 scancode, int extended);
		
		[DllImport("libfreerdp-locale")]
		public static extern UInt32 freerdp_keyboard_get_rdp_scancode_from_virtual_key_code(UInt32 vkcode, ref int extended);
		
		public Keyboard()
		{
			keyboardLayoutId = freerdp_keyboard_init(0);
		}
		
		public UInt32 GetRdpScancodeFromX11Keycode(UInt32 keycode, ref int extended)
		{
			UInt32 scancode;			
			scancode = freerdp_keyboard_get_rdp_scancode_from_x11_keycode(keycode, ref extended);
			return scancode;
		}
		
		public UInt32 GetX11KeycodeFromRdpScancode(UInt32 scancode, int extended)
		{
			UInt32 keycode;
			keycode = freerdp_keyboard_get_x11_keycode_from_rdp_scancode(scancode, extended);
			return keycode;
		}
		
		public UInt32 GetRdpScancodeFromVirtualKeyCode(UInt32 vkcode, ref int extended)
		{
			UInt32 scancode;
			scancode = freerdp_keyboard_get_rdp_scancode_from_virtual_key_code(vkcode, ref extended);
			return scancode;
		}
		
		~Keyboard()
		{

		}
	}
}
