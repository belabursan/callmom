using System;
using System.Threading;
using System.Threading.Tasks;

namespace CallMomCore
{
	public interface INetworkClient
	{
		Task SendAsync (byte[] data, CancellationToken token = default(CancellationToken));

		void Dispose ();

		bool Disconnected ();
	}
}

