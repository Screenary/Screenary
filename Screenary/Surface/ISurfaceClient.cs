using System;
using System.IO;

namespace Screenary
{
	public interface ISurfaceClient
	{
		bool OnSurfaceCommand(BinaryReader s);
	}
}

