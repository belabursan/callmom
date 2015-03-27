using System;

namespace CallMomCore
{
	public class MomSqlException : Exception
	{
		public MomSqlException
		(string message = default(String), Exception innerEx = default(Exception)) : base (message, innerEx)
		{
		}

		public static MomSqlException ToMomException (Exception ex, string method = default(string))
		{
			string message = U.InnerExMessage (ex);
			System.Diagnostics.Debug.WriteLine ("[SQL] - {0}() exception: {1}", (method ?? "?"), message);

			return ex.GetType () == typeof(MomSqlException) ? (MomSqlException)ex : new MomSqlException (message);
		}
	}
}

