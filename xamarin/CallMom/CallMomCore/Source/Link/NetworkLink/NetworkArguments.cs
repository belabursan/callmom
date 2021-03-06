﻿using System;

namespace CallMomCore
{

	public class MomLinger
	{
		public int Timeout{ get; set; }

		public bool Enable { get; set; }
	}

	public class NetworkArguments
	{
		public string LocalIp { get; set; }

		public int LocalPort { get; set; }

		public string Ip { get; set; }

		public int Port { get; set; }

		public int SendTimeoutSeconds { get; set; }

		public int ReceiveTimeoutSeconds { get; set; }

		public MomLinger LingerArguments{ get; set; }

		public bool NoDelay { get; set; }

		public int ConnectTimeoutSeconds { get; set; }
	}
}

