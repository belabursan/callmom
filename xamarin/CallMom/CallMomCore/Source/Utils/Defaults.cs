using System;

namespace CallMomCore
{
	public static class Defaults
	{
		public const string IP = "127.0.0.1";
		//public const string IP = "192.168.0.100";
		public const int PORT = 2015;
		public const int NETTIMEOUT = 8;
		public const int CALLTIMEOUT = 30;
		public const int CONNECTTIMEOUT = 8000;
		public const int BLOCKSIZE = 16;
		public const int KEYSIZE = 32;
		public const string PADDING = "{";
		public static readonly byte[] AESIV = new byte[] {
			0x42, 0x75, 0x72, 0x73, 0x61, 0x6e, 0x42, 0x65,
			0x6c, 0x61, 0x4c, 0x61, 0x73, 0x7a, 0x6c, 0x6f
		};
	}
}

