using System;

namespace CallMomCore
{
	public interface ICryptoFactory
	{
		string GetMD5Hash (string data);

		string EncodeAES (string key, string data, int blockSize, string padding);
	}
}

