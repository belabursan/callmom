using System;

namespace CallMomCore
{
	public class MomException : Exception
	{
		protected MomException (string message = default(string), Exception ex = default(Exception))
			: base (message, ex)
		{
		}
	}
}

