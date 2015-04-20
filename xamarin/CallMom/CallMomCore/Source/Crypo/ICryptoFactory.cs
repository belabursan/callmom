using System;

namespace CallMomCore
{
	public interface ICryptoFactory
	{
		byte[] GetSha256Hash (string data);

		string EncodeAES (byte[] key, string data);

		string EncodeRSA (byte[] key, byte[] data);
	}
}

