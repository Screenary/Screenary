/**
 * Screenary: Real-Time Collaboration Redefined.
 * Config File
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Screenary.Client
{
	public class Config
	{
		[XmlAttribute("BroadcasterPort")]
		public int BroadcasterPort { get; set; }
		
		[XmlAttribute("BroadcasterHostname")]
		public string BroadcasterHostname { get; set; }
		
		[XmlAttribute("BroadcasterAutoconnect")]
		public bool BroadcasterAutoconnect { get; set; }
		
		[XmlAttribute("RdpServerPort")]
		public int RdpServerPort { get; set; }
		
		[XmlAttribute("RdpServerHostname")]
		public string RdpServerHostname { get; set; }
		
		[XmlAttribute("RdpServerUsername")]
		public string RdpServerUsername { get; set; }
		
		[XmlAttribute("RdpServerDomain")]
		public string RdpServerDomain { get; set; }
		
		[XmlAttribute("RdpServerPassword")]
		public string RdpServerPassword { get; set; }
		
		public Config()
		{
			BroadcasterPort = 4489;
			BroadcasterHostname = "localhost";
			BroadcasterAutoconnect = true;
			
			RdpServerPort = 3389;
			RdpServerHostname = "localhost";
			RdpServerUsername = "Administrator";
			RdpServerDomain = "";
			RdpServerPassword = "";
		}
		
		public static void Save(Config config)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Config));
			TextWriter textWriter = new StreamWriter(@"config.xml");
			serializer.Serialize(textWriter, config);
			textWriter.Close();
		}
		
		public static Config Load()
		{
			Config config;
			
			if (File.Exists(@"config.xml"))
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(Config));
				TextReader textReader = new StreamReader(@"config.xml");
				config = (Config) deserializer.Deserialize(textReader);
				textReader.Close();
			}
			else
			{
				config = new Config();
				Save(config);
			}
			
			return config;
		}
	}
}

