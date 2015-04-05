using System;

namespace CallMomCore
{
	public class MomSqlException : MomException
	{
		public static int UNKNOWN = 0;
		public static int NOT_FOUND = -1;

		public int ErrorCode { get; set; }

		public MomSqlException
		(string message = default(String), Exception innerEx = default(Exception), int errorCode = default(int)) : base (message, innerEx)
		{
			ErrorCode = errorCode;
		}

		public static MomSqlException ToMomException (Exception ex, string method = default(string))
		{
			string message = U.InnerExMessage (ex);
			int code = ToErrorCode (ex);
			System.Diagnostics.Debug.WriteLine ("[SQL] - {0}() exception[{1}]: {2}", (method ?? "?"), code, message);

			return ex.GetType () == typeof(MomSqlException) ? (MomSqlException)ex : new MomSqlException (message, default(Exception), code);
		}

		private static int ToErrorCode (Exception ex)
		{
			int code = UNKNOWN;
			if (ex.GetType () == typeof(InvalidOperationException)) {
				code = NOT_FOUND;
			}

			return code;
		}
	}
}

