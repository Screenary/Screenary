using System;
using System.IO;

namespace Screenary
{
	public class Pcap
	{
		protected Stream stream;
		protected pcap_header_t pcap_header;
		protected pcap_record_t pcap_record;
		public const uint MAGIC_NUMBER = 0xA1B2C3D4;
		
		protected struct pcap_header_t
		{
			public UInt32 magic_number;		/* magic number */
			public UInt16 version_major;	/* major version number */
			public UInt16 version_minor;	/* minor version number */
			public Int32 thiszone;			/* GMT to local correction */
			public UInt32 sigfigs;			/* accuracy of timestamps */
			public UInt32 snaplen;			/* max length of captured packets, in octets */
			public UInt32 network;			/* data link type */
		};
		
		protected struct pcap_record_t
		{
			public UInt32 ts_sec;		/* timestamp seconds */
			public UInt32 ts_usec;		/* timestamp microseconds */
			public UInt32 incl_len;		/* number of octets of packet saved in file */
			public UInt32 orig_len;		/* actual length of packet */
			public byte[] buffer;		/* record buffer */
		};
		
		public Pcap(Stream stream)
		{
			this.stream = stream;
		}
	}
}

