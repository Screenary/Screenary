using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Screenary;

namespace Broadcaster
{
	public class Receiver
	{
		/* Client Socket */
		private TcpClient client;
		private Thread thread;
		
		/* Linked list implementation of FIFO queue, should replace with real queue... LoL */
		private LinkedList<PDU> pduQ = new LinkedList<PDU>();
		private readonly object lockQ = new object();
		private bool usingQ = false;
		
		/**
		 * Class constructor
		 */ 
		public Receiver(TcpClient client)
		{
			this.client = client;
			thread = new Thread(threadFunction);
			thread.Start();
		}
		
		/**
		 * Enqueue a PDU
		 */
		public void addPDU(PDU pdu)
		{
			enqueue(pdu);
		}
		
		/**
		 * Thread function continuously dequeues PDU queue and sends to client
		 */ 
		private void threadFunction()
		{
			PDU pdu = null;
			while (true)
			{
				while (pduQ.Count > 0)
				{
					pdu = dequeue();
					sendPDU(pdu);
				}
			}
		}
		
		/**
		 * Send a PDU to client. Buffer is fragmented, and sent with prefixed header
		 */ 
		private void sendPDU(PDU pdu)
		{
			byte[] header = new byte[PDU.PDU_HEADER_LENGTH];
			header[0] = BitConverter.GetBytes(PDU.PDU_CHANNEL_UPDATE)[0];
			header[1] = BitConverter.GetBytes(PDU.PDU_CHANNEL_UPDATE)[1];
			header[2] = PDU.PDU_UPDATE_SURFACE;
			
			NetworkStream stream = client.GetStream();
			byte[] buffer = pdu.getRecord().Buffer;
			int offset = 0;

			/* Fragment the PCAP Record and send through transport */
			
			/* Single fragment */
			if (buffer.Length <= PDU.PDU_MAX_PAYLOAD_SIZE)
			{
				Console.WriteLine("Single");
				header[3] = PDU.PDU_FRAGMENT_SINGLE;
				header[4] = BitConverter.GetBytes(Convert.ToUInt16(buffer.Length))[0];
				header[5] = BitConverter.GetBytes(Convert.ToUInt16(buffer.Length))[1];
				
				stream.Write(header, 0, PDU.PDU_HEADER_LENGTH);
				stream.Write(buffer, offset, buffer.Length);
				return;
			}
			else
			{
				/* First fragment of a series of fragments */
				Console.WriteLine("First");
				header[3] = PDU.PDU_FRAGMENT_FIRST;
				header[4] = BitConverter.GetBytes(PDU.PDU_MAX_PAYLOAD_SIZE)[0];
				header[5] = BitConverter.GetBytes(PDU.PDU_MAX_PAYLOAD_SIZE)[1];
				
				stream.Write(header, 0, PDU.PDU_HEADER_LENGTH);
				stream.Write(buffer, offset, PDU.PDU_MAX_PAYLOAD_SIZE);
				offset += PDU.PDU_MAX_PAYLOAD_SIZE;
				
				while (offset < buffer.Length)
				{					
					if ((offset + PDU.PDU_MAX_PAYLOAD_SIZE) > (buffer.Length))
					{
						/* Last fragment of a series of fragments */
						Console.WriteLine("Last");
						UInt16 size = Convert.ToUInt16((buffer.Length) - offset);
						header[3] = PDU.PDU_FRAGMENT_LAST;
						header[4] = BitConverter.GetBytes(size)[0];
						header[5] = BitConverter.GetBytes(size)[1];
						
						stream.Write(header, 0, PDU.PDU_HEADER_LENGTH);
						stream.Write(buffer, offset, (buffer.Length - offset));
					}
					else
					{
						/* "In between" fragment of a series of fragments */
						Console.WriteLine("Next");
						header[3] = PDU.PDU_FRAGMENT_NEXT;
						header[4] = BitConverter.GetBytes(PDU.PDU_MAX_PAYLOAD_SIZE)[0];
						header[5] = BitConverter.GetBytes(PDU.PDU_MAX_PAYLOAD_SIZE)[1];
												
						stream.Write(header, 0, PDU.PDU_HEADER_LENGTH);
						stream.Write(buffer, offset, PDU.PDU_MAX_PAYLOAD_SIZE);
					}
					
					offset += PDU.PDU_MAX_PAYLOAD_SIZE;
				}
			}			
		}
		
		/**
		 * Enqueue a PDU
		 */ 
		private void enqueue(PDU pdu)
		{
			wait();
			pduQ.AddLast(pdu);
			signal();
		}
		
		/**
		 * Dequeue a PDU and return the object
		 */ 
		private PDU dequeue()
		{
			wait();
			PDU pdu = pduQ.First.Value;
			pduQ.RemoveFirst();
			signal();
			return pdu;
		}
		
		/**
		 * Block on lock until it's released
		 */ 
		public void wait()
		{
			lock (lockQ)
			{
				while (usingQ == true) Monitor.Wait(lockQ);
				usingQ = true;
			}
		}
		
		/**
		 * Release lock
		 */ 
		public void signal()
		{
			lock (lockQ)
			{
				usingQ = false;
				Monitor.Pulse(lockQ);
			}
		}

		private byte[] combine(byte[] a, byte[] b)
		{
			byte[] c = new byte[a.Length + b.Length];
			System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
			System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
			return c;
		}
	}
}
