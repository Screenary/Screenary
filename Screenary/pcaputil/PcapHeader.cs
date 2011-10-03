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
			return String.Format ("\nmagicNumber = {0:x8}" + "\nversionMajor = {1}" + "\nversionMinor = {2}" + "\nthiszone = {3}" + "\nsigfigs = {4}" + "\nsnaplen = {5}" + "\nnetwork = {6}", this.magicNumber, this.versionMajor, this.versionMinor, this.thiszone, this.sigfigs, this.snaplen, this.network);
		}
		
	}
	
}
