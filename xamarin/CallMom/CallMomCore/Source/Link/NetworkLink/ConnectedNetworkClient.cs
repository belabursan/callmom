using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Diagnostics;

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

		public void Close ()
		{
			try {
				if (_stream != null) {
					_stream.Flush ();
					_stream.Dispose ();
				}
			} catch (Exception x) {
				Debug.WriteLine ("ending client");
			}
		}

		public async Task SendAsync (string data, CancellationToken token = default(CancellationToken))
		{
			byte[] b = data.AsBytes ();
			await _stream.WriteAsync (b, 0, b.Length, token);
			await _stream.FlushAsync (token);
		}


		public async Task<string> ReceiveAsync (CancellationToken token = default(CancellationToken))
		{
			byte[] bytes = await ReceiveAsBytesAsync (token);
			return bytes.AsString (0, bytes.Length);
		}


		public async Task<byte[]> ReceiveAsBytesAsync (CancellationToken token = default(CancellationToken))
		{
			byte[] buffer = new byte[8192];
			int dataReaded = await _stream.ReadAsync (buffer, 0, buffer.Length, token);
			byte[] bytes = new byte[dataReaded];
			Array.Copy (buffer, bytes, dataReaded);
			return bytes;
		}

		#endregion

	}
}

