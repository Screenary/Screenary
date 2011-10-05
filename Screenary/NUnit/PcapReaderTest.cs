using System;
using System.IO;
using NUnit.Framework;

namespace Screenary
{
	[TestFixture()]
	public class PcapReaderTest
	{
		[Test()]
		public void IteratorTest()
		{
			int count = 0;
			byte[] buffer_A = new byte[16];
			byte[] buffer_B = new byte[32];
			byte[] buffer_C = new byte[64];
			byte[][] buffers = new byte[3][];
			
			buffers[0] = buffer_A;
			buffers[1] = buffer_B;
			buffers[2] = buffer_C;

			for (int i = 0; i < buffer_A.Length; i++)
				buffer_A[i] = 0xAA;
			
			for (int i = 0; i < buffer_B.Length; i++)
				buffer_B[i] = 0xBB;
			
			for (int i = 0; i < buffer_C.Length; i++)
				buffer_C[i] = 0xCC;
			
			string filename = "data/test.pcap";		
			PcapReader pcap = new PcapReader(filename);
			
			foreach (PcapRecord record in pcap)
			{
				//Assert.Equals(record.Buffer, buffers[count]);
				count++;
			}
			
			Assert.AreEqual(count, 3);
		}
	}
}

