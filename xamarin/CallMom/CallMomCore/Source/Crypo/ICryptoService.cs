using System;

namespace CallMomCore
{
	public interface ICryptoService
	{
		string GetRandomString (int lengt = Defaults.BLOCKSIZE);

		string GenerateHash (string data);

		string CreateRSAKey (string data, int blockSize, string padding);

		string EncodeRSA (string key, string data);

		string DecodeRSA (string key, string data);

		string EncodeAES (string key, string data, int blockSize, string padding);

		string DecodeAES (string key, string data);
	}
}

