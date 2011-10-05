
using System;
using System.IO;
using NUnit.Framework;

namespace Screenary
{
	[TestFixture()]
	public class SurfaceCommandTest
	{
		[Test()]
		public void ParseTest()
		{
			BinaryReader fp;
			string filename;
			//SurfaceCommand cmd;
			
			filename = "data/rfx/rfx.bin";
			fp = new BinaryReader(File.Open(filename, FileMode.Open));
			
			//cmd = SurfaceCommand.Parse(fp);
			
			//Assert.Equals(cmd.cmdType, SurfaceCommand.CMDTYPE_STREAM_SURFACE_BITS);
			
			fp.Close();
		}
	}
}

