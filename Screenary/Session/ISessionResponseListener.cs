using System;

namespace Screenary
{
	public interface ISessionResponseListener
	{	
		void OnRecvJoinRsp(char[] sessionKey);
		void OnRecvLeaveRsp();
		void OnRecvAuthRsp();
		void OnRecvCreateRsp(char[] sessionKey);
		void OnRecvTermRsp(char[] sessionKey);	
	}
}

