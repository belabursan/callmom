using System;

namespace CallMomCore
{
	public class MomArgumentException : MomException
	{
		public int ErrorCode { get; set; }

		public MomArgumentException (int errorCode = default(int), string message = default(string), Exception ex = default(Exception))
			: base (message, ex)
		{
			ErrorCode = errorCode;
		}
	}
}

