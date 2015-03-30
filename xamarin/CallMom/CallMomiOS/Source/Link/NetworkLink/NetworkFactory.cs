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
		public NetworkFactory ()
		{
		}

		private void ConnectCallBack (IAsyncResult asyn)
		{
			//Console.WriteLine ("[NET-Factory] - 1e++++++++++++++++++");
			TcpClient client = asyn.AsyncState as TcpClient;
			if (client != null && client.Client != null) {
				try {
					client.EndConnect (asyn);
				} catch (Exception ex) {
					Console.WriteLine ("[NET-Factory] - 2e++++++++++++++++++");
				}
			}
		}

		#region INetworkFactory implementation

		public async Task<IConnectedNetworkClient> Connect (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{
			try {
				bool connected = true;
				Socket socket = GetSocket (netArgs, token);	

				await Task.Run (async () => {
					IAsyncResult result = socket.BeginConnect (GetEndpoint (netArgs), new AsyncCallback (ConnectCallBack), socket);
					connected = result.AsyncWaitHandle.WaitOne (netArgs.ConnectTimeout, true);
				});
					
				if (!connected && socket != null && !socket.Connected) {
					socket.Close ();
				}

				return new ConnectedNetworkClient (new NetworkStream (socket, FileAccess.ReadWrite, true));
			
			} catch (Exception ex) {
				Console.WriteLine ("[NET-Factory] - exception when connecting ({0}:{1})", U.ExType (ex), U.InnerExMessage (ex));
				throw;
			}
		}


		#endregion

		private static LingerOption ToLingerOption (NetworkArguments netArgs, CancellationToken token)
		{
			ThrowIfCancelled (token);
			//return new LingerOption (netArgs.LingerArguments.Enable, netArgs.LingerArguments.Timeout);
			return new LingerOption (false, 0);
		}

		private static Socket GetSocket (NetworkArguments netArgs, CancellationToken token = default(CancellationToken))
		{

			var client = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			client.ReceiveTimeout = netArgs.ReceiveTimeout;
			client.SendTimeout = netArgs.SendTimeout;
			client.LingerState = ToLingerOption (netArgs, token);
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

