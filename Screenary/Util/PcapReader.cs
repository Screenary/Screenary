/**
 * Screenary: Real-Time Collaboration Redefined.
 * Pcap Reader Utils
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
using System.IO;
using System.Collections;

namespace Screenary
{
	public class PcapReader : Pcap, IEnumerable
	{
		private BinaryReader reader;
		private int seconds_offset = -1;
		
		public PcapReader(Stream stream) : base(stream)
		{
			reader = new BinaryReader(stream);
			ReadHeader();
		}
		
		public PcapReader(string filename) : this(File.OpenRead(filename))
		{
				
		}
		
		private void ReadHeader()
		{
			pcap_header.magic_number = reader.ReadUInt32();
			pcap_header.version_major = reader.ReadUInt16();
			pcap_header.version_minor = reader.ReadUInt16();
			pcap_header.thiszone = reader.ReadInt32();
			pcap_header.sigfigs = reader.ReadUInt32();
			pcap_header.snaplen = reader.ReadUInt32();
			pcap_header.network = reader.ReadUInt32();
		}
		
		private PcapRecord GetNext()
		{
			long ticks;
			int sec, usec;
			PcapRecord record;
			
			pcap_record.ts_sec = reader.ReadUInt32();
			pcap_record.ts_usec = reader.ReadUInt32();
			pcap_record.incl_len = reader.ReadUInt32();
			pcap_record.orig_len = reader.ReadUInt32();
			pcap_record.buffer = reader.ReadBytes((int) pcap_record.incl_len);
			
			sec = (int) pcap_record.ts_sec;
			usec = (int) pcap_record.ts_usec;
			
			if (seconds_offset < 0)
				seconds_offset = sec;
			
			sec -= seconds_offset;
			
			ticks = sec * TimeSpan.TicksPerSecond;
			ticks += (usec / 1000) * TimeSpan.TicksPerMillisecond;

			record = new PcapRecord(pcap_record.buffer, new TimeSpan(ticks));
			
			return record;
		}
		
		private bool HasNext()
		{
			return (reader.BaseStream.Position < reader.BaseStream.Length);
		}
		
		public IEnumerator GetEnumerator()
		{
			while (this.HasNext())
			{
				yield return this.GetNext();
			}
		}
		
		public void Close()
		{
			reader.Close();
		}
	}
}

