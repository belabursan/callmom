using System;

namespace CallMomCore
{
	public static class U
	{
		public static string InnerExMessage (Exception exception)
		{
			Exception tmpEx = InnerEx (exception);
			if (tmpEx != null) {
				return tmpEx.Message + string.Empty;
			}
			return string.Empty;
		}

		public static Exception InnerEx (Exception exception)
		{
			Exception tmpEx = exception.InnerException;
			while (tmpEx != null && tmpEx.InnerException != null) {
				tmpEx = tmpEx.InnerException;
			}
			return tmpEx ?? exception;
		}

		public static String ExType (Exception exception)
		{
			return InnerEx (exception).GetType ().Name + String.Empty;
		}

		/*
		public static T Throw<T> (string methodName, Exception ex) where T : Exception
		{
			return (T)new Exception (
				String.Format ("Exception in {0}(): {1}", methodName, U.InnerExMessage (ex)),
				U.InnerEx (ex)
			);

		}
		*/
	}
}

