using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace CallMomCore
{
	public interface INetworkLink
	{
		Task<IConnectedNetworkClient> GetNewConnection (NetworkArguments netArgs, CancellationToken token = default(CancellationToken));
	}
}

