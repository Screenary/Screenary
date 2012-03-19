using System;

namespace Screenary
{
	public class TransportException : Exception
	{
		public TransportException () : base()
		{
			
		}
		
		public TransportException(string message) : base(message)
		{
				
		}
	}
}

