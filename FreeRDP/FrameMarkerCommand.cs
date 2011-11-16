using System;
using System.IO;

namespace FreeRDP
{
	public class FrameMarkerCommand : SurfaceCommand
	{
		private UInt16 frameAction;
		private UInt32 frameId;
		
		public FrameMarkerCommand()
		{
		}
		
		public override void Read(BinaryReader fp)
		{
			frameAction = fp.ReadUInt16(); /* frameAction */
			frameId = fp.ReadUInt32(); /* frameId */
		}
		
		public override void Execute(SurfaceReceiver receiver)
		{
			//receiver.window.ProcessUpdates(false);	
		}
	}
}

