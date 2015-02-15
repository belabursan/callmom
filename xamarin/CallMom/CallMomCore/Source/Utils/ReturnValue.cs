using System;

namespace CallMomCore
{
	public static class ReturnValue
	{
		public static int Success { get { return 0; } }

		public static int NotRegistered { get { return 1; } }

		public static int AlreadyRunning { get { return 2; } }

		public static int Cancelled { get { return 3; } }

		public static int Error { get { return 4; } }

		public static int NotRunning { get { return 5; } }

		public static int NetworkError { get { return 6; } }
	}
}

