using System;
using System.Collections;
using System.IO;

namespace Screenary.Server
{
	public class ScreenSessions
	{
		
   	    private static ScreenSessions instance;
		static readonly object padlock = new object();

		private Hashtable screensessions; 
		private static UInt32 sessionId = 100;
		
		public ScreenSessions ()
		{
			this.screensessions= new Hashtable(); 
			
		}
		
		public static ScreenSessions Instance
	    {
	    	get 
	      	{
				//lock for multithreading support
				lock (padlock)
            	{
		        	if (instance == null)
		         	{
		            	instance = new ScreenSessions();
		         	}
		         	return instance;
				}
	      	}
	   	}
		
		public ScreencastingSession getScreenSession(string sessionKey)
		{
			if(this.screensessions.ContainsKey(sessionKey))
				return (ScreencastingSession) this.screensessions[sessionKey];
			else
				return null;
		}
		
		//return the new sessionkey and the userId for this session
		public string createScreenSession(string password)
		{
			string newKey = this.GenerateUniqueKey();
			uint userId = this.GenerateUniqueId();
			this.screensessions.Add(newKey, new ScreencastingSession(password, newKey.ToCharArray(), userId));
			//should return as struct instead
			return newKey+"_"+userId.ToString();
		}
		
		public UInt32 terminateScreenSession(string sessionKey, int userid)
		{
			ScreencastingSession s = (ScreencastingSession) this.screensessions[sessionKey];
			//make sure sender id and user that sent the terminate request is the same otherwise dont allow
			if(s.senderId == userid)
			{
				return s.Terminate();
			}
			else 
			{
				return 1;
			}
		}
		
		public string joinScreenSession(string sessionKey, string username, string password)
		{
			if(this.screensessions.ContainsKey(sessionKey))
			{
				ScreencastingSession s = (ScreencastingSession) this.screensessions[sessionKey];
				
				if(!s.isTerminated)
				{
					UInt32 id = this.GenerateUniqueId();
					return s.Authenticate(id, password);
				}
				else
				{
					//session is terminated
					return "-2";
				}
			}
			else
			{
				//session does not exist
				return "-3";				
			}
				
		}
		
		public uint leaveScreenSession(string sessionKey, UInt32 userid)
		{
			if(this.screensessions.ContainsKey(sessionKey))
			{
				ScreencastingSession s = (ScreencastingSession) this.screensessions[sessionKey];							
				return s.RemoveReceiver(userid);				
			}
			else
			{
				//session does not exist
				return 0;				
			}
				
		}
		
		private string GenerateUniqueKey()
		{
			Console.WriteLine("ScreenSessions.GenerateUniqueKey");
			string random = this.GetRandomString();
			while(this.screensessions.ContainsKey(random))
				random = this.GetRandomString();
			
			return random;
		}
		
	    private string GetRandomString()
	    {
			string path = Path.GetRandomFileName();
			return path.Replace(".", "").Substring(0,8); 			
    	}
		
		private UInt32 GenerateUniqueId()
		{
			Console.WriteLine("ScreenSessions.GenerateUniqueId");
			return sessionId++;
		}	
		
		
	}
}

