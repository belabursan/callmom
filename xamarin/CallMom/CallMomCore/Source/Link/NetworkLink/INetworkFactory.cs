using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace CallMomCore
{
	public interface INetworkFactory
	{
		Task<IConnectedNetworkClient> Connect (NetworkArguments netArgs, CancellationToken token = default(CancellationToken));
	}
}

