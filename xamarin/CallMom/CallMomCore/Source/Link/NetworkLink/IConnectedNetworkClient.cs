using System;
using System.Threading.Tasks;
using System.Threading;

namespace CallMomCore
{
	public interface IConnectedNetworkClient
	{
		Task Send (string data, CancellationToken token = default(CancellationToken));

		Task<string> Receive (CancellationToken token = default(CancellationToken));
	}
}

