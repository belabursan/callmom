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

		protected async Task<string> DoHandshake (IConnectedNetworkClient client, CancellationToken token)
		{
			Debug.WriteLine ("[BaseCommand] - starting handshake");

			await client.SendAsync ("hello", token);
			string answer = await client.ReceiveAsync (token);
			var answerList = answer.Split (':');

			if (answerList != null && answerList.Length != 2) {
				throw new MomProtocolException ("Not welcome!");
			}
			//Debug.WriteLine ("[Call] -  received: " + answerList [0] + ":" + answerList [1].Trim ());

			var answerHeader = answerList [0].Trim ();
			if (!"welcome".Equals (answerHeader)) {
				string message = String.Empty;
				if ("failed".Equals (answerHeader)) {
					message = " Server returned \"failed\". Did I sent the right command?";
				} else {
					message = String.Format ("Server returned {0}", answerHeader);
				}
				throw new MomProtocolException (String.Format ("Not welcome!{0}", message));
			}
			Debug.WriteLine ("[BaseCommand] - handshake: {0}", answerHeader);
			return answerList [1];
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

