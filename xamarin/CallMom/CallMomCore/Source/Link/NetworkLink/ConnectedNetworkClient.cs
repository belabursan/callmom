using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace CallMomCore
{
	public class ConnectedNetworkClient : IConnectedNetworkClient
	{
		private Stream _stream;

		public ConnectedNetworkClient (Stream stream)
		{
			_stream = stream;
		}

		#region IConnectedNetworkClient implementation

		public async Task Send (string data, CancellationToken token = default(CancellationToken))
		{
			byte[] b = Encoding.UTF8.GetBytes (data);
			
			await _stream.WriteAsync (b, 0, b.Length);
		}

		#endregion

	}
}

