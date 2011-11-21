using System;
using System.Runtime.InteropServices;

namespace FreeRDP
{
	public unsafe delegate void pBeginPaint(rdpUpdate* update);
	public unsafe delegate void pEndPaint(rdpUpdate* update);
	public unsafe delegate void pSetBounds(rdpUpdate* update, IntPtr bounds);
	public unsafe delegate void pSynchronize(rdpUpdate* update);
	public unsafe delegate void pDesktopResize(rdpUpdate* update);
	public unsafe delegate void pBitmapUpdate(rdpUpdate* update, IntPtr bitmap);
	public unsafe delegate void pPalette(rdpUpdate* update, IntPtr palette);
	public unsafe delegate void pPlaySound(rdpUpdate* update, IntPtr playSound);
	
	public unsafe delegate void pPointerPosition(rdpUpdate* update, IntPtr pointerPosition);
	public unsafe delegate void pPointerSystem(rdpUpdate* update, IntPtr pointerSystem);
	public unsafe delegate void pPointerColor(rdpUpdate* update, IntPtr pointerColor);
	public unsafe delegate void pPointerNew(rdpUpdate* update, IntPtr pointerNew);
	public unsafe delegate void pPointerCached(rdpUpdate* update, IntPtr pointerCached);
	
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct rdpUpdate
	{
		public rdpContext* context;
		public fixed UInt32 paddingA[16-1];
		
		public IntPtr BeginPaint;
		public IntPtr EndPaint;
		public IntPtr SetBounds;
		public IntPtr Synchronize;
		public IntPtr DesktopResize;
		public IntPtr BitmapUpdate;
		public IntPtr Palette;
		public IntPtr PlaySound;
		public fixed UInt32 paddingB[32-24];
		
		public IntPtr PointerPosition;
		public IntPtr PointerSystem;
		public IntPtr PointerColor;
		public IntPtr PointerNew;
		public IntPtr PointerCached;
		public fixed UInt32 paddingC[48-37];
	}
}

