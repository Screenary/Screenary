using System;
using Screenary;
using System.IO;
using System.Text;
using System.Threading;

namespace Screenary.Server
{
	class MainClass
	{
		public static string replayFile;
		public static bool replayMode = false;
		public static string WorkingDirectory;
		
		public static void Main(string[] args)
		{
			WorkingDirectory = Directory.GetCurrentDirectory();
			
			if (!WorkingDirectory.EndsWith("/"))
				WorkingDirectory += "/";
			
			if ((args.Length > 1) && (args[0].Equals("--replay")))
			{
				replayFile = args[1].ToString();
				Console.WriteLine("Replay Mode, using {0}", replayFile);
				replayMode = true;
			}
			
			Broadcaster server = new Broadcaster("127.0.0.1", 4489);
			
			System.Threading.Thread.Sleep(50000);
			Console.WriteLine("50 sec passed");
			
			if (replayMode)
			{
				PcapReader pcap = new PcapReader(File.OpenRead(replayFile));
				
				foreach (PcapRecord record in pcap)
				{
					PDU pdu = new PDU(record.Buffer, 0, 1);
					server.addPDU(pdu, "ABCDEF123456".ToCharArray());
				}
			}
			
			while (true)
			{
				Thread.Sleep(10);
			}
		}
	}
}
