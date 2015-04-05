using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace CallMomCore
{
	public class ConnectedNetworkClient : IConnectedNetworkClient
	{
		private readonly Stream _stream;

		public ConnectedNetworkClient (Stream stream)
		{
			_stream = stream;
		}

		#region IConnectedNetworkClient implementation

		public async Task Send (string data, CancellationToken token = default(CancellationToken))
		{
			byte[] b = data.AsBytes ();
			
			await _stream.WriteAsync (b, 0, b.Length);
		}


		public async Task<string> Receive (CancellationToken token = default(CancellationToken))
		{
			byte[] buffer = new byte[1024];
			int dataReaded = await _stream.ReadAsync (buffer, 0, buffer.Length);
			return buffer.AsString (0, dataReaded);
		}

		#endregion

	}
}

