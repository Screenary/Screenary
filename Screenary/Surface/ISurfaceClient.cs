using System;
using System.IO;

namespace Screenary
{
	public interface ISurfaceClient
	{
		void OnSurfaceCommand(BinaryReader s);
	}
}

