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
		private Thread thread = null;
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
		
		public void StartThread()
		{
			if (thread == null)
			{
				thread = new Thread(() => ThreadProc(this));
			}
			
			thread.Start();
		}
		
		public void StopThread()
		{

		}
		
		public bool Connect(string hostname, Int32 port)
		{
			this.hostname = hostname;
			this.port = port;
	
			if (tcpClient == null)
				tcpClient = new TcpClient();
			
			tcpClient.Connect(this.hostname, this.port);
			
			this.StartThread();
			
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
		
		public void SendPDU(byte[] buffer, UInt16 channelId, byte pduType)
		{
			Socket socket;
			BinaryWriter s;
			int offset = 0;
			UInt16 fragSize;
			int totalSize = 0;
		
			Monitor.Enter(this);
			
			socket = tcpClient.Client;
			totalSize = (int) buffer.Length;
			byte[] fragment = new byte[PDU_MAX_FRAG_SIZE];
			
			if (totalSize <= PDU_MAX_PAYLOAD_SIZE)
			{
				/* Single fragment */
				
				fragSize = (UInt16) totalSize;
				s = new BinaryWriter(new MemoryStream(fragment));
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU_FRAGMENT_SINGLE);
				s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
				s.Write(buffer, 0, fragSize);
				
				SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				return;
			}
			else
			{
				/* First fragment of a series of fragments */
				
				fragSize = (UInt16) PDU_MAX_PAYLOAD_SIZE;
				s = new BinaryWriter(new MemoryStream(fragment));
				
				s.Write(channelId);
				s.Write(pduType);
				s.Write(PDU_FRAGMENT_FIRST);
				s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
				s.Write(buffer, 0, fragSize);
				
				SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
				offset += fragSize;
				
				while (offset < totalSize)
				{
					if ((totalSize - offset) <= PDU_MAX_PAYLOAD_SIZE)
					{
						/* Last fragment of a series of fragments */
						
						fragSize = (UInt16) (totalSize - offset);
						s = new BinaryWriter(new MemoryStream(fragment));
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU_FRAGMENT_LAST);
						s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
						s.Write(buffer, offset, fragSize);
						
						SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
						
						return;
					}
					else
					{
						/* "in between" fragment of a series of fragments */
						
						fragSize = PDU_MAX_PAYLOAD_SIZE;
						s = new BinaryWriter(new MemoryStream(fragment));
						
						s.Write(channelId);
						s.Write(pduType);
						s.Write(PDU_FRAGMENT_NEXT);
						s.Write((UInt16) (fragSize + PDU_HEADER_SIZE));
						s.Write(buffer, offset, fragSize);
						
						SendAll(socket, fragment, 0, fragSize + PDU_HEADER_SIZE);
						offset += fragSize;
					}
				}
			}
			
			Monitor.Exit(this);
			
			return;
		}
		
		public void RecvPDU()
		{
			Socket socket;
			BinaryReader s;
			byte pduType = 0;
			int totalSize = 0;
			byte fragFlags = 0;
			UInt16 channelId = 0;
			UInt16 fragSize = 0;
			byte[] buffer = null;
			
			Monitor.Enter(this);
			
			byte[] header = new byte[PDU_HEADER_SIZE];
			
			if (tcpClient.GetStream().DataAvailable)
			{
				socket = tcpClient.Client;
				
				while (true)
				{
					RecvAll(socket, header, 0, PDU_HEADER_SIZE);
					s = new BinaryReader(new MemoryStream(header));
					
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
						
						dispatcher.DispatchPDU(buffer, channelId, pduType);
					}
					else if (fragFlags == PDU_FRAGMENT_FIRST)
					{
						/* the first of a series of fragments */
						
						buffer = new byte[fragSize];
						RecvAll(socket, buffer, 0, fragSize);
						totalSize = fragSize;
					}
					else if (fragFlags == PDU_FRAGMENT_NEXT)
					{
						/* the "in between" of a series of fragments */
						
						Array.Resize<byte>(ref buffer, totalSize + fragSize);
						RecvAll(socket, buffer, totalSize, fragSize);
						totalSize += fragSize;
					}
					else if (fragFlags == PDU_FRAGMENT_LAST)
					{
						/* The last of a series of fragments */
						
						Array.Resize<byte>(ref buffer, totalSize + fragSize);
						RecvAll(socket, buffer, totalSize, fragSize);
						totalSize += fragSize;
						
						dispatcher.DispatchPDU(buffer, channelId, pduType);
					}
					else
					{
						Console.WriteLine("Invalid Fragmentation Flags: {0}", fragFlags);
						return;
					}
				}
			}
			
			Monitor.Exit(this);
			
			return;
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

