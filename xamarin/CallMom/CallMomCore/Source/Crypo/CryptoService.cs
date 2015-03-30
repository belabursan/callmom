using System;
using System.Linq;


namespace CallMomCore
{
	public class CryptoService : ICryptoService
	{
		private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private static Random _random;
		private readonly ICryptoFactory _factory;

		public CryptoService (ICryptoFactory factory)
		{
			_factory = factory;
			_random = new Random ((int)DateTime.Now.Ticks);
		}

		public string GetRandomString (int lengt = Defaults.BLOCKSIZE)
		{
			return new string (
				Enumerable.Repeat (_chars, lengt).Select (s => s [_random.Next (s.Length)]).ToArray ()
			);
		}

		public string GenerateHash (string data)
		{
			return _factory.GetMD5Hash (data);
		}

		public string EncodeRSA (string key, string data)
		{
			return "todo - using System.Security.Cryptography must be used in ios and android, implement sevice";
		}

		public string CreateRSAKey (string data, int blockSize, string padding)
		{
			throw new NotImplementedException ();
		}

		public string DecodeRSA (string key, string data)
		{
			throw new NotImplementedException ();
		}

		public string EncodeAES (string key, string data, int blockSize, string padding)
		{
			throw new NotImplementedException ();
		}

		public string DecodeAES (string key, string data)
		{
			throw new NotImplementedException ();
		}
	}
}

