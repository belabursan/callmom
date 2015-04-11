using System;
using System.Linq;


namespace CallMomCore
{
	public class CryptoService : ICryptoService
	{
		private const string _chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		private static Random _random;
		private readonly ICryptoFactory _factory;

		public CryptoService (ICryptoFactory factory)
		{
			_factory = factory;
			_random = new Random ((int)DateTime.Now.Ticks);
		}

		public string GetRandomString (int length = Defaults.BLOCKSIZE)
		{
			return new string (
				Enumerable.Repeat (_chars, length).Select (s => s [_random.Next (s.Length)]).ToArray ()
			);
		}

		public string GetRandomString (int minLength = Defaults.BLOCKSIZE, int maxLength = Defaults.KEYSIZE)
		{
			return GetRandomString (_random.Next (minLength, maxLength));
		}

		public byte[] GetSha256Hash (string data)
		{
			return _factory.GetSha256Hash (data);
		}

		public string EncodeRSA (byte[] key, string data)
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

		public string EncodeAES (byte[] key, string data)
		{
			return _factory.EncodeAES (key, data);
		}

		public string DecodeAES (string key, string data)
		{
			throw new NotImplementedException ();
		}
	}
}

