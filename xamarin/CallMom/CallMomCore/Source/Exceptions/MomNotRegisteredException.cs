using System;

namespace CallMomCore
{
	public class MomNotRegisteredException : MomException
	{
		public MomNotRegisteredException (
			string message = default(String),
			Exception innerEx = default(Exception)
		) : base (message, innerEx)
		{
		}
	}
}

