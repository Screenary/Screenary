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

