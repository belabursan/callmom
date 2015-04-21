using System;
using System.Threading;

namespace CallMomCore
{
	public interface ICryptoFactory
	{
		byte[] GetSha256Hash (string data, CancellationToken token = default(CancellationToken));

		string EncodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));

		string EncodeRSA (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));

		string DecodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken));
	}
}

