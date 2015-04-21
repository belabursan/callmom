using System;
using System.Threading;

namespace CallMomCore
{
	public interface ICryptoService
	{
		string GetRandomString (int length = Defaults.BLOCKSIZE, CancellationToken token = default(CancellationToken));

		string GetRandomString (
			int minLength = Defaults.BLOCKSIZE,
			int maxLength = Defaults.KEYSIZE,
			CancellationToken token = default(CancellationToken));

		byte[] GetSha256Hash (string data, CancellationToken token = default(CancellationToken));

		string EncodeRSA (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));

		string EncodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));

		string DecodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));
	}
}

