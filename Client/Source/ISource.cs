using System;
using System.IO;
using FreeRDP;

namespace Screenary.Client
{
	public interface ISource
	{
		void OnSurfaceCommand(SurfaceCommand cmd);
	}
}

