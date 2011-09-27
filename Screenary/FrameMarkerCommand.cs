using System;
using System.IO;

namespace Screenary
{
	public class FrameMarkerCommand : SurfaceCommand
	{
		public UInt16 frameAction;
		public UInt32 frameId;
		
		public FrameMarkerCommand()
		{
		}
		
		public FrameMarkerCommand(UInt16 cmdType)
		{
			this.cmdType = cmdType;
		}
		
		public override void Read(BinaryReader fp)
		{
			frameAction = fp.ReadUInt16(); /* frameAction */
			frameId = fp.ReadUInt32(); /* frameId */
		}
		
		public override void Process()
		{

		}
	}
}

