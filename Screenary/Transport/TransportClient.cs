using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Screenary
{
	public class TransportClient
	{
		private Int32 port;
		private string hostname;
		private TcpClient tcpClient;
		private ChannelDispatcher dispatcher;
		
		private const int PDU_HEADER_SIZE = 6;
		
		public TransportClient(TcpClient tcpClient)
		{
			this.dispatcher = null;
			this.tcpClient = tcpClient;
		}
		
		public TransportClient(ChannelDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			this.tcpClient = null;
		}
		
		public TransportClient(ChannelDispatcher dispatcher, TcpClient tcpClient)
		{
			this.dispatcher = dispatcher;
			this.tcpClient = tcpClient;
		}
		
		public void SetChannelDispatcher(ChannelDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}
		
		public bool Connect(string hostname, Int32 port)
		{
			this.hostname = hostname;
			this.port = port;
	
			if (tcpClient == null)
				tcpClient = new TcpClient();
			
			tcpClient.Connect(this.hostname, this.port);
	
			return true;
		}
		
		public bool Disconnect()
		{
			tcpClient.Close();	
			return true;
		}
		
		public bool SendPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			int offset = 0;
			UInt16 fragSize;
			int totalSize = 0;
			
			BinaryWriter s;
			MemoryStream fstream;
			
			totalSize = (int) buffer.Length;
			byte[] fragment = new byte[PDU.PDU_MAX_FRAG_SIZE];
			fstream = new MemoryStream(fragment);
			s = new BinaryWriter(fstream);
			
			if (totalSize <= PDU.PDU_MAX_PAYLOAD_SIZE)
			{
				/* Single fragment */
				
				Console.WriteLine("Single");

				fragSize = (UInt16) totalSize;
				fstream.Seek(0, SeekOrigin.Begin);
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU.PDU_FRAGMENT_SINGLE);
				s.Write(fragSize);
				s.Write(buffer);
	
				tcpClient.GetStream().Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				return true;
			}
			else
			{
				/* First fragment of a series of fragments */
				Console.WriteLine("First");
				
				fragSize = (UInt16) PDU.PDU_MAX_PAYLOAD_SIZE;
				fstream.Seek(0, SeekOrigin.Begin);
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU.PDU_FRAGMENT_FIRST);
				s.Write(fragSize);
				s.Write(buffer, offset, fragSize);

				tcpClient.GetStream().Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				while (offset < buffer.Length)
				{					
					if ((totalSize - offset) <= PDU.PDU_MAX_PAYLOAD_SIZE)
					{
						/* Last fragment of a series of fragments */
						Console.WriteLine("Last");
						
						fragSize = (UInt16) (totalSize - offset);
						fstream.Seek(0, SeekOrigin.Begin);
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU.PDU_FRAGMENT_LAST);
						s.Write(fragSize);
						s.Write(buffer, offset, fragSize);
						
						tcpClient.GetStream().Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
						
						return true;
					}
					else
					{
						/* "In between" fragment of a series of fragments */
						Console.WriteLine("Next");
						
						fragSize = PDU.PDU_MAX_PAYLOAD_SIZE;
						fstream.Seek(0, SeekOrigin.Begin);
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU.PDU_FRAGMENT_NEXT);
						s.Write(fragSize);
						s.Write(buffer, offset, fragSize);
						
						tcpClient.GetStream().Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
					}
				}
			}
			
			return true;
		}
		
		public bool RecvPDU()
		{
			byte pduType = 0;
			int totalSize = 0;
			byte fragFlags = 0;
			UInt16 channelId = 0;
			UInt16 fragSize = 0;
			byte[] buffer = null;
			BinaryReader s;
			
			while (tcpClient.GetStream().DataAvailable)
			{
				s = new BinaryReader(tcpClient.GetStream());
				
				channelId = s.ReadUInt16();
				pduType = s.ReadByte();
				fragFlags = s.ReadByte();
				fragSize = s.ReadUInt16();
				
				//fragSize -= PDU_HEADER_SIZE;
				
				Console.WriteLine("PDU channelId:{0} pduType:{1} fragFlags:{2} fragSize:{3}",
					channelId, pduType, fragFlags, fragSize);
				
				if (fragSize <= 0)
					continue;
				
				if (fragFlags == PDU.PDU_FRAGMENT_SINGLE)
				{
					/* a single fragment */
					
					Console.WriteLine("PDU_FRAGMENT_SINGLE");
					
					buffer = new byte[fragSize];
					s.Read(buffer, 0, fragSize);
					totalSize = fragSize;
					
					return dispatcher.DispatchPDU(buffer, channelId, pduType);
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_FIRST)
				{
					/* the first of a series of fragments */
					
					Console.WriteLine("PDU_FRAGMENT_FIRST");
					
					buffer = new byte[fragSize];
					s.Read(buffer, 0, fragSize);
					totalSize = fragSize;
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_NEXT)
				{
					/* the "in between" of a series of fragments */
				
					Console.WriteLine("PDU_FRAGMENT_NEXT");
					
					Array.Resize(ref buffer, totalSize + fragSize);
					s.Read(buffer, totalSize, fragSize);
					totalSize += fragSize;
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_LAST)
				{
					/* The last of a series of fragments */
					
					Console.WriteLine("PDU_FRAGMENT_LAST");
					
					Array.Resize(ref buffer, totalSize + fragSize);
					s.Read(buffer, totalSize, fragSize);
					totalSize += fragSize;
					
					return dispatcher.DispatchPDU(buffer, channelId, pduType);
				}
				
				Thread.Sleep(10);
			}
			
			return true;
		}
	}
}

