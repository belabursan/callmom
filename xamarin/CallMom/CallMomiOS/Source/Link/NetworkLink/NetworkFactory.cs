using CallMomCore;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System;

namespace CallMomiOS
{
	public class NetworkFactory : INetworkFactory
	{
		private TcpClient _TCPClient;

		public NetworkFactory ()
		{
			_TCPClient = null;
		}

		#region INetworkFactory implementation

		public async Task<Stream> Connect (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			try {
				if (_TCPClient != null) {
					_TCPClient.Close ();
					_TCPClient = null;
				}

				token.ThrowIfCancellationRequested ();
				_TCPClient = new TcpClient ();
				token.ThrowIfCancellationRequested ();

				_TCPClient.NoDelay = netArgs.NoDelay;
				_TCPClient.LingerState = ToLingerOption (netArgs);
				_TCPClient.ReceiveTimeout = netArgs.ReceiveTimeout;
				_TCPClient.SendTimeout = netArgs.SendTimeout;

				token.ThrowIfCancellationRequested ();
				//SocketAsyncEventArgs vv = new SocketAsyncEventArgs();
				//_TCPClient.Client.c

				await _TCPClient.ConnectAsync (ToIp (netArgs), netArgs.Port);
				token.ThrowIfCancellationRequested ();

				return _TCPClient.GetStream ();
			} catch (Exception ex) {
				Console.WriteLine ("[NET-Factory] - exception when connecting ({0}:{1})", U.ExType (ex), U.InnerExMessage (ex));
				Disconnect ();
				throw;
			}

		}

		public void Disconnect ()
		{
			if (_TCPClient != null) {
				try {
					_TCPClient.Close ();
				} catch (Exception ex) {
					Console.WriteLine ("[NET-Factory] - ignoring exception when closing({0}:{1})", U.ExType (ex), U.InnerExMessage (ex));
					//ignore exception when closing client
				} finally {
					_TCPClient = null;
				}
			}
		}

		#endregion

		private static LingerOption ToLingerOption (NetworkArguments netArgs)
		{
			return new LingerOption (netArgs.LingerArguments.Enable, netArgs.LingerArguments.Timeout);
		}

		private static IPAddress ToIp (NetworkArguments netArgs)
		{
			return IPAddress.Parse (netArgs.Ip);
		}

	}
}

