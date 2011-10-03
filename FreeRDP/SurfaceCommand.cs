using System;
using System.IO;

namespace FreeRDP
{	
	public abstract class SurfaceCommand
	{
		private const UInt16 CMDTYPE_SET_SURFACE_BITS = 1;
		private const UInt16 CMDTYPE_STREAM_SURFACE_BITS = 6;
		private const UInt16 CMDTYPE_FRAME_MARKER = 4;
		
		public SurfaceCommand()
		{
		}
		
		public virtual void Read(BinaryReader fp) {}
		public virtual void Execute(Gdk.Window window, Gdk.Pixbuf surface) {}
		
		public static SurfaceCommand Parse(BinaryReader fp)
		{
			UInt16 cmdType;
			SurfaceCommand cmd = null;
			
			cmdType = fp.ReadUInt16();
			
			switch (cmdType)
			{
				case CMDTYPE_SET_SURFACE_BITS:
					cmd = new SetSurfaceBitsCommand();
					cmd.Read(fp);
					break;
				
				case CMDTYPE_STREAM_SURFACE_BITS:
					cmd = new StreamSurfaceBitsCommand();
					cmd.Read(fp);
					break;
				
				case CMDTYPE_FRAME_MARKER:
					cmd = new FrameMarkerCommand();
					cmd.Read(fp);
					break;
			}
			
			return cmd;
		}
	}
}

