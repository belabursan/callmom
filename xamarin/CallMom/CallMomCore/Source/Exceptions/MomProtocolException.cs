using System;

namespace CallMomCore
{
	public class MomProtocolException : MomException
	{
		public MomProtocolException (string message = default(String),
		                             Exception innerEx = default(Exception)
		) : base (message, innerEx)
		{
		}
	}
}

