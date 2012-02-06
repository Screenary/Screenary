using System;
using System.IO;

namespace Screenary
{
	public interface ISurfaceServer
	{
		void OnSurfaceCommand(UInt32 sessionId, byte[] surfaceCommand);
	}
}

