using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Autofac;

namespace CallMomCore
{
	public abstract class CallBase : ICommand
	{
		protected readonly ISettingsService _settings;
		protected readonly ICryptoService _cryptoService;
		protected readonly IFileService _fileService;
		private readonly INetworkLink _network;
		protected readonly CancellationTokenSource _cancelation;

		protected CallBase ()
		{
			_settings = App.Container.Resolve<ISettingsService> ();
			_cryptoService = App.Container.Resolve<ICryptoService> ();
			_fileService = App.Container.Resolve<IFileService> ();
			_network = App.Container.Resolve<INetworkLink> ();
			_cancelation = new CancellationTokenSource ();
		}

		public virtual int Cancel ()
		{
			Debug.WriteLine ("[BaseCommand] - cancelling the call");

			_cancelation.Cancel ();
			Debug.WriteLine ("[BaseCommand] - cancelled");

			return ReturnValue.Cancelled;
		}

		public virtual async Task<int> ExecuteAsync (string value = default(string))
		{
			Debug.WriteLine ("[BaseCommand] - executing command");
			try {
				return await Run (value, _cancelation.Token);
			} catch (OperationCanceledException ocex) {
				Debug.WriteLine ("[BaseCommand] - cancelled (got OperationCanceledException:{0})", U.InnerExMessage (ocex));
				return ReturnValue.Cancelled;
			} catch (MomNotRegisteredException nrex) {
				Debug.WriteLine ("[BaseCommand] - not registered (got NotRegisteredException:{0})", U.InnerExMessage (nrex));
				return ReturnValue.NotRegistered;
			} catch (MomNetworkException neex) {
				Debug.WriteLine ("[BaseCommand] - network error (got MomNetworkException:{0})", U.InnerExMessage (neex));
				return ReturnValue.NetworkError;
			} catch (Exception ex) {
				Debug.WriteLine ("[BaseCommand] - general exception {0}({1})", U.ExType (ex), U.InnerExMessage (ex));
				return ReturnValue.Error;
			}

		}

		/// <summary>
		/// Performs the handshake to the server and gets the version number of the protocol.
		/// </summary>
		/// <returns>The version of the protocol as string in format x.x.x</returns>
		/// <param name="client">Connected network client holding the socket</param>
		/// <param name="token">Cancellation Token.</param>
		protected async Task<string> DoHandshake (IConnectedNetworkClient client, CancellationToken token)
		{
			Debug.WriteLine ("[BaseCommand] - starting handshake");

			await client.SendAsync (Protocol.HELLO, token);
			string answer = await client.ReceiveAsync (token);
			return ValidateAnswer (answer);
		}

		protected async Task<byte[]> SendKey (IConnectedNetworkClient client, CancellationToken token)
		{
			Debug.WriteLine ("[BaseCommand] - starting send key");

			byte[] aesKey = _cryptoService.GetSha256Hash (_cryptoService.GetRandomString (512));
			string crKey = _cryptoService.EncodeRSA (_settings.GetServerPublicKey (), aesKey);
			string command = String.Format ("{0}{1}{2}", Protocol.XCHANGEKEY, Protocol.SPLITTER, crKey);
			await client.SendAsync (command, token);
			string answer = await client.ReceiveAsync (token);
			ValidateAnswer (answer);
			return aesKey;

		}

		private static string ValidateAnswer (string answer)
		{
			if (String.IsNullOrEmpty (answer)) {
				throw new MomProtocolException ("Illegal answer");
			}

			var answerList = answer.Split (Protocol.SPLITTER);
			switch (answerList [0].Trim ()) {
			case Protocol.WELCOME:
				if (answerList.Length > 1) {
					return answerList [1].Trim ();
				} else {
					throw new MomProtocolException ("Missing version");
				}
			case Protocol.SUCCESS:
				return Protocol.SUCCESS;
			case Protocol.FAILED:
				if (answerList.Length > 1) {
					throw new MomProtocolException (String.Format ("Failed: {0}", answerList [1]));
				} else {
					throw new MomProtocolException (String.Format ("Failed"));
				}
			case Protocol.EXIT:
				throw new MomProtocolException (Protocol.EXIT);
			default:
				throw new MomProtocolException ("Wrong answer");
			}
		}

		protected async Task<IConnectedNetworkClient> Connect (CancellationToken token)
		{
			NetworkArguments netArgs = ValidateValues (token);
			return await _network.GetNewConnection (netArgs, token);
		}

		private NetworkArguments ValidateValues (CancellationToken token)
		{
			#if !DEVEL
			if (_settings.GetIP ().Equals (Defaults.IP) == true) {
				throw new MomNotRegisteredException (String.Format ("IP is no valid ({0}", Defaults.IP));
			}
			#endif

			NetworkArguments netArg = new NetworkArguments ();

			token.ThrowIfCancellationRequested ();
			netArg.Ip = _settings.GetIP ();
			netArg.Port = _settings.GetPort ();

			int connectTimeout = _settings.GetConnectTimeOut ();
			int netTimeout = _settings.GetNetworkTimeoutSeconds ();

			if (connectTimeout > netTimeout) {
				netArg.ReceiveTimeoutSeconds = netTimeout;
				netArg.SendTimeoutSeconds = netTimeout;
			} else {
				netArg.ReceiveTimeoutSeconds = connectTimeout;
				netArg.SendTimeoutSeconds = connectTimeout;
			}

			netArg.NoDelay = true;
			netArg.LingerArguments = new MomLinger {
				Enable = false,
				Timeout = 0
			};
			netArg.ConnectTimeoutSeconds = _settings.GetConnectTimeOut ();

			return netArg;
		}


		protected abstract Task<int> Run (string value = default(string), CancellationToken token = default(CancellationToken));
	}
}

