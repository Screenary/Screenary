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
			RdpServerPassword = "Password123!";
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

