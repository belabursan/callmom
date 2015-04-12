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

		private void ConnectCallBack (IAsyncResult asyn)
		{
			//Console.WriteLine ("[NET-Factory] - 1e++++++++++++++++++");
			TcpClient client = asyn.AsyncState as TcpClient;
			if (client != null && client.Client != null) {
				try {
					client.EndConnect (asyn);
				} catch (Exception ex) {
					Console.WriteLine ("[NET-Factory] - ex: {0}", ex.Message);
				}
			}
		}

		#region INetworkFactory implementation

		public async Task<IConnectedNetworkClient> Connect (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			try {
				bool connected = true;
				Socket socket = GetSocket (netArgs, token);	

				await Task.Run (() => {
					IAsyncResult result = socket.BeginConnect (GetEndpoint (netArgs), new AsyncCallback (ConnectCallBack), socket);
					connected = result.AsyncWaitHandle.WaitOne (netArgs.ConnectTimeoutSeconds * 1000, true);
				});
					
				if (!connected && socket != null && !socket.Connected) {
					socket.Close ();
				}

				return new ConnectedNetworkClient (new NetworkStream (socket, FileAccess.ReadWrite, true));
			
			} catch (Exception ex) {
				//Console.WriteLine ("[NET-Factory] - exception when connecting ({0}:{1})", U.ExType (ex), U.InnerExMessage (ex));
				throw MomNetworkException.ToMomException (ex, "[NET-Factory] - Connect");
			}
		}

		#endregion

		private static Socket GetSocket (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{

			var client = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			client.ReceiveTimeout = netArgs.ReceiveTimeoutSeconds;
			client.SendTimeout = netArgs.SendTimeoutSeconds;
			client.LingerState = new LingerOption (netArgs.LingerArguments.Enable, netArgs.LingerArguments.Timeout);
			client.NoDelay = netArgs.NoDelay;
			ThrowIfCancelled (token);

			return client;
		}

		private static IPEndPoint GetEndpoint (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			ThrowIfCancelled (token);
			return new IPEndPoint (IPAddress.Parse (netArgs.Ip), netArgs.Port);
		}

		private static void ThrowIfCancelled (CancellationToken token)
		{
			if (token != default(CancellationToken)) {
				token.ThrowIfCancellationRequested ();
			}
		}
	}
}

