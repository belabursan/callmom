using System;
using System.Linq;
using System.Threading;


namespace CallMomCore
{
	public class CryptoService : ICryptoService
	{
		private const string _chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTVWXYZ";
		private readonly ICryptoFactory _factory;

		public CryptoService (ICryptoFactory factory)
		{
			_factory = factory;

		}

		public string GetRandomString (int length = Defaults.BLOCKSIZE, CancellationToken token = default(CancellationToken))
		{
			if (token != default(CancellationToken))
				token.ThrowIfCancellationRequested ();
			
			var random = new Random ((int)DateTime.Now.Ticks);
			return new string (
				Enumerable.Repeat (_chars, length).Select (s => s [random.Next (s.Length)]).ToArray ()
			);
		}

		public string GetRandomString (
			int minLength = Defaults.BLOCKSIZE,
			int maxLength = Defaults.KEYSIZE,
			CancellationToken token = default(CancellationToken))
		{
			if (token != default(CancellationToken))
				token.ThrowIfCancellationRequested ();
			
			var random = new Random ((int)DateTime.Now.Ticks);
			return GetRandomString (random.Next (minLength, maxLength), token);
		}

		public byte[] GetSha256Hash (string data, CancellationToken token = default(CancellationToken))
		{
			if (token != default(CancellationToken))
				token.ThrowIfCancellationRequested ();
			
			return _factory.GetSha256Hash (data);
		}

		public string EncodeRSA (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			return _factory.EncodeRSA (key, data, token);
		}

		public string EncodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			return _factory.EncodeAES (key, data, token);
		}

		public string DecodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			return _factory.DecodeAES (key, data, token);
		}
	}
}

