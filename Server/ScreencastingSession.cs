using System;
using System.Collections.Generic;

namespace Screenary.Server
{
	public class ScreencastingSession
	{
		public Boolean isTerminated { get; set; }
		public String password { get; set; }
		public char[] key { get; set; }
		public UInt32 senderId { get; set; }
		private List<UInt32> receiverIds = new List<UInt32>();
		
		public ScreencastingSession ( string password, char[] key, UInt32 senderId)
		{
			this.password = password;
			this.key = key;
			this.senderId = senderId;
		}
		
		private UInt32 AddReceiver(UInt32 receiverId)
		{
			UInt32 sessionStatus = 1;
			if(!isTerminated)
			{
				if(!receiverIds.Contains(receiverId))
				{
					receiverIds.Add(receiverId);
					sessionStatus = 0;
				}
			}
			return sessionStatus;
		}
		
		public UInt32 RemoveReceiver(UInt32 receiverId)
		{
			UInt32 sessionStatus = 1;
			if(!isTerminated)
			{
				if(receiverIds.Remove(receiverId))
				{
					sessionStatus = 0;
				}
			}
			return sessionStatus;
		}
		
		public string Authenticate(UInt32 receiverId, String password)
		{
			UInt32 sessionStatus = 1;
			if(!isTerminated)
			{
				if(this.password == password)
				{
					this.AddReceiver(receiverId);
					return receiverId.ToString();
				}
				else
				{
					//authentication failed
					return "-4";					
				}
			}
			return "-2";
		}
		
		public UInt32 Terminate()
		{
			UInt32 sessionStatus = 1;
			if(!isTerminated)
			{
				isTerminated = true;
				sessionStatus = 0;
			}
			return sessionStatus;
		}
	}
}

