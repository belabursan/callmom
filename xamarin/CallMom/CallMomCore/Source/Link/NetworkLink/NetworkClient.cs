using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace CallMomCore
{
	public class NetworkClient : INetworkClient
	{
		private readonly Stream _Stream;

		public NetworkClient (Stream stream)
		{
			_Stream = stream;
		}

		#region INetworkClient implementation

		public async Task SendAsync (byte[] data, CancellationToken token = default(CancellationToken))
		{
			await _Stream.WriteAsync (data, 0, data.Length, token);
			await _Stream.FlushAsync (token);
		}

		public void Dispose ()
		{
			if (_Stream != null)
				_Stream.Dispose ();
		}

		public bool Disconnected ()
		{
			if (_Stream != null && _Stream.CanRead && _Stream.CanSeek && _Stream.CanWrite) {
				return false;
			}
			return true;
		}

		#endregion
	}
}

