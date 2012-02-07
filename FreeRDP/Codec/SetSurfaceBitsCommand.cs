using System;
using System.IO;

namespace FreeRDP
{
	public class SetSurfaceBitsCommand : SurfaceBitsCommand
	{		
		public SetSurfaceBitsCommand() : base()
		{

		}
		
		public override UInt16 GetCmdType()
		{
			return CMDTYPE_SET_SURFACE_BITS;
		}
	}
}
