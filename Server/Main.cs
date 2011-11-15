using System;
using System.Text;
using System.Threading;
using Screenary;
using System.IO;

namespace Screenary.Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{				
			BroadcasterServer server = new BroadcasterServer("127.0.0.1", 12680);
			
			PcapReader pcap = new PcapReader(File.OpenRead("../../data/ferrari.pcap"));
			foreach (PcapRecord record in pcap)
			{
				PDU pdu = new PDU(record);
				server.addPDU(pdu);
			}
			
			while (true)
			{
				Thread.Sleep(10);
			}
		}
	}
}
