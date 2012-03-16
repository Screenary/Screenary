/**
 * Screenary: Real-Time Collaboration Redefined.
 * Main
 *
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
			
			Broadcaster broadcaster = new Broadcaster("127.0.0.1", 4489);
			
			/*
			if (replayMode)
			{
				PcapReader pcap = new PcapReader(File.OpenRead(replayFile));
				
				foreach (PcapRecord record in pcap)
				{
					PDU pdu = new PDU(record.Buffer, 0, 1);
					server.addPDU(pdu, "ABCDEF123456".ToCharArray());
				}
			}*/
			
			broadcaster.MainLoop();
		}
	}
}
