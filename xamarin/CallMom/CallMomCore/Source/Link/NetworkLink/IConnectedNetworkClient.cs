using System;
using System.Threading.Tasks;
using System.Threading;

namespace CallMomCore
{
	public interface IConnectedNetworkClient
	{
		Task SendAsync (string data, CancellationToken token = default(CancellationToken));

		Task<string> ReceiveAsync (CancellationToken token = default(CancellationToken));

		Task<byte[]> ReceiveAsBytesAsync (CancellationToken token = default(CancellationToken));
	}
}

