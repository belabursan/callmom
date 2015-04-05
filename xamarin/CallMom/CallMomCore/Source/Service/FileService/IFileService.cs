using System;
using System.Threading;
using System.Threading.Tasks;

namespace CallMomCore
{
	public interface IFileService
	{
		Task SaveFileAsync (byte[] file, string name, CancellationToken token = default(CancellationToken));

		Task<byte[]> GetFileAsync (string name, CancellationToken token = default(CancellationToken));

		Task SavePublicKeyAsync (byte[] file, CancellationToken token = default(CancellationToken));

		Task<byte[]> GetPublicKeyAsync (CancellationToken token = default(CancellationToken));

	}
}

