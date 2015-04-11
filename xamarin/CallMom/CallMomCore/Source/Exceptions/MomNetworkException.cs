using System;
using System.IO;

namespace CallMomCore
{
	public class MomNetworkException : MomException
	{
		public static int UNKNOWN = 0;
		public static int NOT_CONNECTED = 1;

		public int ErrorCode { get; set; }

		public MomNetworkException (
			string message = default(String),
			Exception innerEx = default(Exception),
			int errorCode = default(int)
		) : base (message, innerEx)
		{
			ErrorCode = errorCode;
		}

		public static Exception Throw (string method, Exception ex)
		{
			return new MomNetworkException (
				String.Format ("Exception in {0}(): {1}", method, U.InnerExMessage (ex)),
				U.InnerEx (ex)
			);
		}



		public static MomNetworkException ToMomException (Exception ex, string method = default(string))
		{
			string message = U.InnerExMessage (ex);
			int code = ToErrorCode (ex);
			System.Diagnostics.Debug.WriteLine ("[SQL] - {0}() exception[{1}]: {2}", (method ?? "?"), code, message);

			return ex.GetType () == typeof(MomNetworkException) ? (MomNetworkException)ex : new MomNetworkException (message, default(Exception), code);
		}

		private static int ToErrorCode (Exception ex)
		{
			int code = UNKNOWN;
			if (ex.GetType () == typeof(IOException)) {
				code = NOT_CONNECTED;
			}

			return code;
		}

	}
}

