/**
 * FreeRDP: A Remote Desktop Protocol Implementation
 * Surface Receiver
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
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

namespace FreeRDP
{
	public class SurfaceReceiver
	{
		public Rfx rfx;
		public Gdk.Window window;
		public Gdk.Pixbuf surface;
		
		public SurfaceReceiver(Gdk.Window window, Gdk.Pixbuf surface)
		{
			rfx = new Rfx();
			this.window = window;
			this.surface = surface;
		}
		
		public void InvalidateRect(int x, int y, int width, int height)
		{			
			window.InvalidateRect(new Gdk.Rectangle(x, y, 64, 64), true);
		}
	}
}

