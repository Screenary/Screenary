using System;
namespace Screenary
{
	public class PcapHeader
	{
		private uint _magicNumber;   /* magic number */
		private uint _versionMajor;  /* major version number */
		private uint _versionMinor;  /* minor version number */
		private int _thiszone;       /* GMT to local correction */
		private uint _sigfigs;       /* accuracy of timestamps */
		private uint _snaplen;       /* max length of captured packets, in octets */
		private uint _network;       /* data link type */
	
		public PcapHeader ()
		{
		}
		
		public uint MagicNumber
		{
			get
			{
				return _magicNumber;
			}
			
			set
			{
				_magicNumber = value;
			}
		}

		public uint VersionMajor
		{
			get
			{
				return _versionMajor;
			}
			
			set
			{
				_versionMajor = value;
			}
		}

		public uint VersionMinor
		{
			get
			{
				return _versionMinor;
			}
			
			set
			{
				_versionMinor = value;
			}
		}

		public int Thiszone
		{
			get
			{
				return _thiszone;
			}
			
			set
			{
				_thiszone = value;
			}
		}

		public uint Sigfigs
		{
			get
			{
				return _sigfigs;
			}
			
			set
			{
				_sigfigs = value;
			}
		}

		public uint Snaplen
		{
			get
			{
				return _snaplen;
			}
			
			set
			{
				_snaplen = value;
			}
		}

		public uint Network
		{
			get
			{
				return _network;
			}
			
			set
			{
				_network = value;
			}
		}

		public String toString()
		{
			return String.Format("\nmagicNumber = {0:x8}" + "\nversionMajor = {1}" + "\nversionMinor = {2}" + 
								"\nthiszone = {3}" + "\nsigfigs = {4}" + "\nsnaplen = {5}" + "\nnetwork = {6}", 
			                     this.MagicNumber, this.VersionMajor, this.VersionMinor, this.Thiszone,
			               		 this.Sigfigs, this.Snaplen, this.Network);
		}
						
	}
		
}
