using System;
using System.IO;

namespace Screenary
{	
	public class SurfaceCommand
	{
		public UInt16 cmdType;
		public static RemoteFX remotefx = null;
		
		public const UInt16 CMDTYPE_SET_SURFACE_BITS = 1;
		public const UInt16 CMDTYPE_STREAM_SURFACE_BITS = 6;
		public const UInt16 CMDTYPE_FRAME_MARKER = 4;
		
		public SurfaceCommand()
		{
			if (remotefx == null)
				remotefx = new RemoteFX();
			
			this.cmdType = 0;
		}
		
		public SurfaceCommand(UInt16 cmdType)
		{
			if (remotefx == null)
				remotefx = new RemoteFX();
			
			this.cmdType = cmdType;
		}
		
		public virtual void Read(BinaryReader fp) { }
		public virtual void Process() { }
		
		public static SurfaceCommand Parse(BinaryReader fp)
		{
			UInt16 cmdType;
			SurfaceCommand cmd = null;
			
			cmdType = fp.ReadUInt16();
			Console.WriteLine("cmdType:" + cmdType);
			
			switch (cmdType)
			{
				case CMDTYPE_SET_SURFACE_BITS:
				case CMDTYPE_STREAM_SURFACE_BITS:
					cmd = new SurfaceBitsCommand(cmdType);
					cmd.Read(fp);
					break;
				
				case CMDTYPE_FRAME_MARKER:
					cmd = new FrameMarkerCommand(cmdType);
					cmd.Read(fp);
					break;
			}
			
			return cmd;
		}
	}
}

