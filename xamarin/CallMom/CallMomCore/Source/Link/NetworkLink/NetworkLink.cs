using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;
using System.Threading;

namespace CallMomCore
{
	public class NetworkLink : INetworkLink
	{
		private readonly INetworkFactory _networkFactory;

		public NetworkLink (INetworkFactory networkFactory)
		{
			_networkFactory = networkFactory;
		}

		#region INetworkLink implementation

		public async Task<IConnectedNetworkClient> GetNewConnection (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			return await _networkFactory.Connect (netArgs, token);
		}

		#endregion
	}
}

