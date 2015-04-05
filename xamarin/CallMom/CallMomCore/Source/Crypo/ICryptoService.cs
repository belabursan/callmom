using System;

namespace CallMomCore
{
	public interface ICryptoService
	{
		string GetRandomString (int length = Defaults.BLOCKSIZE);

		string GetRandomString (int minLength = Defaults.BLOCKSIZE, int maxLength = Defaults.KEYSIZE);

		byte[] GetSha256Hash (string data);

		string CreateRSAKey (string data, int blockSize, string padding);

		string EncodeRSA (string key, string data);

		string DecodeRSA (string key, string data);

		string EncodeAES (byte[] key, string data);

		string DecodeAES (string key, string data);
	}
}

