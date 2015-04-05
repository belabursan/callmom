using System;

namespace CallMomCore
{
	public class MomNetworkException : MomException
	{
		public MomNetworkException (
			string message = default(String),
			Exception innerEx = default(Exception)
		) : base (message, innerEx)
		{
		}

		public static Exception Throw (string method, Exception ex)
		{
			return new MomNetworkException (
				String.Format ("Exception in {0}(): {1}", method, U.InnerExMessage (ex)),
				U.InnerEx (ex)
			);
		}
	}
}

