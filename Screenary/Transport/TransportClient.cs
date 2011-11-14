using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Screenary
{
	public class TransportClient
	{
		private Int32 port;
		private string hostname;
		private TcpClient tcpClient;
		private ChannelDispatcher dispatcher;
		
		public TransportClient(ChannelDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
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
	
			tcpClient.Connect(this.hostname, this.port);
	
			return true;
		}
		
		public bool Disconnect()
		{
			tcpClient.Close();	
			return true;
		}
		
		public bool SendPDU(byte[] buffer, UInt16 channel, byte pduType)
		{		
			return true;
		}
		
		public bool RecvPDU()
		{
			byte pduType = 0;
			int totalSize = 0;
			byte fragFlags = 0;
			UInt16 channel = 0;
			UInt16 fragSize = 0;
			byte[] buffer = null;
			
			BinaryReader s;
			NetworkStream stream;
			
			stream = tcpClient.GetStream();
			s = new BinaryReader(stream);
			
			while (true)
			{
				channel = s.ReadUInt16();
				pduType = s.ReadByte();
				fragFlags = s.ReadByte();
				fragSize = s.ReadUInt16();
				
				if (fragFlags == PDU.PDU_FRAGMENT_SINGLE)
				{
					/* a single fragment */
					
					buffer = new byte[fragSize];
					s.Read(buffer, 0, fragSize);
					totalSize = fragSize;
					
					return dispatcher.DispatchPDU(buffer, channel, pduType);
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_FIRST)
				{
					/* the first of a series of fragments */
					
					buffer = new byte[fragSize];
					s.Read(buffer, 0, fragSize);
					totalSize = fragSize;
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_NEXT)
				{
					/* the "in between" of a series of fragments */
				
					Array.Resize(ref buffer, totalSize + fragSize);
					s.Read(buffer, totalSize, fragSize);
					totalSize += fragSize;
				}
				else if (fragFlags == PDU.PDU_FRAGMENT_LAST)
				{
					/* The last of a series of fragments */
					
					Array.Resize(ref buffer, totalSize + fragSize);
					s.Read(buffer, totalSize, fragSize);
					totalSize += fragSize;
					
					return dispatcher.DispatchPDU(buffer, channel, pduType);
				}
			}
		}
	}
}

