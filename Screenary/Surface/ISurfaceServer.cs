using System;
using System.IO;

namespace Screenary
{
	public interface ISurfaceServer
	{
		void OnSurfaceCommand(char[] sessionKey, byte[] buffer);
	}
}

