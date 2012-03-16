/**
 * Screenary: Real-Time Collaboration Redefined.
 * Pcap Source
 *
 * Copyright 2011-2012 Marc-Andre Moreau <marcandre.moreau@gmail.com>
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
using FreeRDP;
using System.IO;
using System.Threading;

namespace Screenary.Client
{
	public class PcapSource
	{
		private string filename;
		
		private Thread thread;
		private ISource iSource;
	
		public PcapSource(ISource iSource)
		{
			this.iSource = iSource;
			thread = new Thread(() => ThreadProc(this));
		}
		
		public void Play(string filename)
		{
			this.filename = filename;
			thread.Start();
		}
		
		static void ThreadProc(PcapSource pcapSource)
		{
			int count = 0;
			SurfaceCommand cmd;
			MemoryStream stream;
			BinaryReader reader;
			
			PcapReader pcap = new PcapReader(File.OpenRead(pcapSource.filename));
			TimeSpan previousTime = new TimeSpan(0, 0, 0, 0);
			
			foreach (PcapRecord record in pcap)
			{				
				Thread.Sleep(record.Time.Subtract(previousTime));						
				previousTime = record.Time;
				
				stream = new MemoryStream(record.Buffer);
				reader = new BinaryReader(stream);
				
				cmd = SurfaceCommand.Parse(reader);
				pcapSource.iSource.OnSurfaceCommand(cmd);
			}
			
			pcap.Close();
		}
	}
}

