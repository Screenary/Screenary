/**
 * Screenary: Real-Time Collaboration Redefined.
 * Pcap Record Utils
 *
 * Copyright 2011-2012 Terri-Anne Cambridge <tacambridge@gmail.com>
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
	public class PcapRecord
	{
		public byte[] Buffer { get; set; }
		
		public int Length
		{
			get { return Buffer.Length; }
		}
		
		public TimeSpan Time { get; set; }
		
		public PcapRecord(byte[] buffer, TimeSpan time)
		{
			this.Buffer = buffer;
			this.Time = time;
		}
	}
}

