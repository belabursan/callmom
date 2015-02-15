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
		private INetworkClient _Client;

		public NetworkLink (INetworkFactory networkFactory)
		{
			_networkFactory = networkFactory;
			_Client = null;
		}

		#region INetworkLink implementation

		public async Task<INetworkClient> Connect (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			try {
				if (_Client == null || _Client.Disconnected ()) {
					var stream = await _networkFactory.Connect (netArgs, token);
					_Client = new NetworkClient (stream);
				}
					
				return _Client;

			} catch (Exception ex) {
				throw MomNetworkException.Throw ("Connect", ex);
			}
		}

		public void Disconnect ()
		{
			if (_Client != null) {
				_Client.Dispose ();
				_Client = null;
				_networkFactory.Disconnect ();
			}
		}

		#endregion
	}
}

