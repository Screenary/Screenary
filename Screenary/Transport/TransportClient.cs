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
		private Thread thread;
		private TcpClient tcpClient;
		private ChannelDispatcher dispatcher;
		
		public const int PDU_HEADER_SIZE = 6;
		public const int PDU_MAX_FRAG_SIZE = 0x3FFF;
		public const int PDU_MAX_PAYLOAD_SIZE = (PDU_MAX_FRAG_SIZE - PDU_HEADER_SIZE);
		
		public const UInt16 PDU_CHANNEL_SESSION = 0x00;
		public const UInt16 PDU_CHANNEL_UPDATE = 0x01;
		public const UInt16 PDU_CHANNEL_INPUT = 0x02;
		
		public const byte PDU_UPDATE_SURFACE = 0x01;
		
		public const byte PDU_FRAGMENT_SINGLE = 0x00;
		public const byte PDU_FRAGMENT_FIRST = 0x01;
		public const byte PDU_FRAGMENT_NEXT = 0x02;
		public const byte PDU_FRAGMENT_LAST = 0x03;
		
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
			
			thread = new Thread(() => ThreadProc(this));
			thread.Start();
			
			dispatcher.OnConnect();
	
			return true;
		}
		
		public bool Disconnect()
		{
			tcpClient.Close();
			dispatcher.OnDisconnect();
			
			return true;
		}
		
		private void SendAll(Socket socket, byte[] buffer, int offset, int size)
		{
			int sent = 0;
			int total_sent = 0;
			int end = offset + size;
			
			while (offset < end)
			{
				sent = socket.Send(buffer, offset, (size - total_sent), 0);
				total_sent += sent;
				offset += sent;
			}
		}
		
		private void RecvAll(Socket socket, byte[] buffer, int offset, int size)
		{
			int recv = 0;
			int total_recv = 0;
			int end = offset + size;
			
			while (offset < end)
			{
				recv = socket.Receive(buffer, offset, (size - total_recv), 0);
				total_recv += recv;
				offset += recv;
			}
		}
		
		public bool SendPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			int offset = 0;
			UInt16 fragSize;
			int totalSize = 0;
			
			Socket socket;
			BinaryWriter s;
			MemoryStream fstream;
			NetworkStream nstream;
		
			Monitor.Enter(this);
			
			socket = tcpClient.Client;
			totalSize = (int) buffer.Length;
			byte[] fragment = new byte[PDU_MAX_FRAG_SIZE];
			fstream = new MemoryStream(fragment);
			
			if (totalSize <= PDU_MAX_PAYLOAD_SIZE)
			{
				/* Single fragment */
				
				fragSize = (UInt16) totalSize;
				nstream = tcpClient.GetStream();
				fstream.Seek(0, SeekOrigin.Begin);
				s = new BinaryWriter(fstream);
				
				Console.WriteLine("PDU_FRAGMENT_SINGLE: offset:{0} frag:{1} total:{2} after:{3}",
					offset, fragSize, totalSize, offset + fragSize);
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU_FRAGMENT_SINGLE);
				s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
				s.Write(buffer, 0, fragSize);
				
				SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
				//nstream.Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				return true;
			}
			else
			{
				/* First fragment of a series of fragments */
				
				fragSize = (UInt16) PDU_MAX_PAYLOAD_SIZE;
				nstream = tcpClient.GetStream();
				fstream.Seek(0, SeekOrigin.Begin);
				s = new BinaryWriter(fstream);
		
				Console.WriteLine("PDU_FRAGMENT_FIRST: offset:{0} frag:{1} total:{2} after:{3}",
					offset, fragSize, totalSize, offset + fragSize);
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU_FRAGMENT_FIRST);
				s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
				s.Write(buffer, 0, fragSize);
				
				SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
				//nstream.Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				while (offset < totalSize)
				{
					if ((totalSize - offset) <= PDU_MAX_PAYLOAD_SIZE)
					{
						/* Last fragment of a series of fragments */
						
						fragSize = (UInt16) (totalSize - offset);
						nstream = tcpClient.GetStream();
						fstream.Seek(0, SeekOrigin.Begin);
						s = new BinaryWriter(fstream);
						
						Console.WriteLine("PDU_FRAGMENT_LAST: offset:{0} frag:{1} total:{2} after:{3}",
							offset, fragSize, totalSize, offset + fragSize);
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU_FRAGMENT_LAST);
						s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
						s.Write(buffer, offset, fragSize);
						
						SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
						//nstream.Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
						
						return true;
					}
					else
					{
						/* "in between" fragment of a series of fragments */
						
						fragSize = PDU_MAX_PAYLOAD_SIZE;
						nstream = tcpClient.GetStream();
						fstream.Seek(0, SeekOrigin.Begin);
						s = new BinaryWriter(fstream);
						
						Console.WriteLine("PDU_FRAGMENT_NEXT: offset:{0} frag:{1} total:{2} after:{3}",
							offset, fragSize, totalSize, offset + fragSize);
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU_FRAGMENT_NEXT);
						s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
						s.Write(buffer, offset, fragSize);
						
						SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
						//nstream.Write(fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
					}
				}
			}
			
			Monitor.Exit(this);
			
			return true;
		}
		
		public bool RecvPDU()
		{
			Socket socket;
			byte pduType = 0;
			int totalSize = 0;
			byte fragFlags = 0;
			UInt16 channelId = 0;
			UInt16 fragSize = 0;
			byte[] buffer = null;
			NetworkStream nstream;
			MemoryStream mstream;
			BinaryReader s;
			
			Monitor.Enter(this);
			
			byte[] header = new byte[PDU_HEADER_SIZE];
			
			if (tcpClient.GetStream().DataAvailable)
			{
				nstream = tcpClient.GetStream();
				s = new BinaryReader(nstream);
				socket = tcpClient.Client;
				
				while (true)
				{
					RecvAll(socket, header, 0, PDU_HEADER_SIZE);
					mstream = new MemoryStream(header);
					s = new BinaryReader(mstream);
					
					channelId = s.ReadUInt16();
					pduType = s.ReadByte();
					fragFlags = s.ReadByte();
					fragSize = s.ReadUInt16();
					
					fragSize -= PDU_HEADER_SIZE;
					
					if (fragFlags == PDU_FRAGMENT_SINGLE)
					{
						/* a single fragment */
						
						buffer = new byte[fragSize];
						RecvAll(socket, buffer, 0, fragSize);
						totalSize = fragSize;
						
						Console.WriteLine("PDU_FRAGMENT_SINGLE: frag:{0} total:{1}", fragSize, totalSize);
						
						return dispatcher.DispatchPDU(buffer, channelId, pduType);
					}
					else if (fragFlags == PDU_FRAGMENT_FIRST)
					{
						/* the first of a series of fragments */
						
						buffer = new byte[fragSize];
						RecvAll(socket, buffer, 0, fragSize);
						totalSize = fragSize;
						
						Console.WriteLine("PDU_FRAGMENT_FIRST: frag:{0} total:{1}", fragSize, totalSize);
					}
					else if (fragFlags == PDU_FRAGMENT_NEXT)
					{
						/* the "in between" of a series of fragments */
						
						Array.Resize<byte>(ref buffer, totalSize + fragSize);
						RecvAll(socket, buffer, totalSize, fragSize);
						totalSize += fragSize;
						
						Console.WriteLine("PDU_FRAGMENT_NEXT: frag:{0} total:{1}", fragSize, totalSize);
					}
					else if (fragFlags == PDU_FRAGMENT_LAST)
					{
						/* The last of a series of fragments */
						
						Array.Resize<byte>(ref buffer, totalSize + fragSize);
						RecvAll(socket, buffer, totalSize, fragSize);
						totalSize += fragSize;
						
						Console.WriteLine("PDU_FRAGMENT_LAST: frag:{0} total:{1}", fragSize, totalSize);
						
						return dispatcher.DispatchPDU(buffer, channelId, pduType);
					}
					else
					{
						Console.WriteLine("Invalid Fragmentation Flags: {0}", fragFlags);
					}
				}
			}
			
			Monitor.Exit(this);
			
			return true;
		}
		
		static void ThreadProc(TransportClient client)
		{
			while (true)
			{
				client.RecvPDU();
				Thread.Sleep(10);
			}
		}
	}
}

