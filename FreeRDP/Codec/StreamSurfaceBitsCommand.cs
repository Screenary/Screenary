using System;
using System.IO;

namespace FreeRDP
{
	public class StreamSurfaceBitsCommand : SurfaceBitsCommand
	{
		public StreamSurfaceBitsCommand() : base()
		{
			
		}
		
		public override UInt16 GetCmdType()
		{
			return CMDTYPE_STREAM_SURFACE_BITS;
		}
	}
}

