using System;
using System.Linq;




namespace CallMomCore
{
	public class Crypto
	{
		private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private static Random _random;

		public Crypto ()
		{
			_random = new Random ((int)DateTime.Now.Ticks);
		}

		public string GetRandomString (int lengt = 16)
		{
			return new string (
				Enumerable.Repeat (chars, lengt).Select (s => s [_random.Next (s.Length)]).ToArray ()
			);
		}

		public string EncodeRSA (string key, string data)
		{
			return "todo - using System.Security.Cryptography must be used in ios and android, implement sevice";
		}
	}
}

