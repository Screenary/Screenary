using System;
namespace Screenary
{
	public class Packet
	{

		private byte[] _data;
		private uint _length;
		
		public Packet ()
		{
		}
				
		public byte[] Data
		{
			get
			{
				return _data;
			}
			
			set
			{
				_data = value;
			}
		}
		
		public uint Length
		{
			get
			{
				return _length;
			}
			
			set
			{
				_length = value;
			}
		}		
	}
}

