using System;

namespace Screenary
{
	public class PcapHeader
	{
		public uint magicNumber { get; set; }	/* magic number */
		public uint versionMajor { get; set; }	/* major version number */		
		public uint versionMinor { get; set; }	/* minor version number */
		public int thiszone { get; set; }		/* GMT to local correction */
		public uint sigfigs { get; set; }		/* accuracy of timestamps */
		public uint snaplen { get; set; }		/* max length of captured packets, in octets */
		public uint network { get; set; }		/* data link type */

		public String toString()
		{			
			return String.Format("magicNumber = {0:x8}\n" + "versionMajor = {1}\n" + "versionMinor = {2}\n" +
				"thiszone = {3}\n" + "sigfigs = {4}\n" + "snaplen = {5}\n" + "network = {6}\n",
				this.magicNumber, this.versionMajor, this.versionMinor,
				this.thiszone, this.sigfigs, this.snaplen, this.network);
		}
	}
}
