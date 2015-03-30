using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Autofac;
using System.Threading;

namespace CallMomCore
{
	public class Call
	{
		private readonly ISettingsService _settings;
		private readonly CancellationTokenSource _cancelation;
		private readonly ICryptoService _cryptoService;


		public Call ()
		{
			_settings = App.Container.Resolve<ISettingsService> ();
			_cryptoService = App.Container.Resolve<ICryptoService> ();
			_cancelation = new CancellationTokenSource ();
		}

		public async Task<int> Execute ()
		{
			Debug.WriteLine ("[Call] - executing call");
			try {
				return await Run (_cancelation.Token);	
			} catch (OperationCanceledException ocex) {
				Debug.WriteLine ("[Call] - cancelled (got OperationCanceledException:{0})", U.InnerExMessage (ocex));
				return ReturnValue.Cancelled;
			} catch (MomNotRegisteredException nrex) {
				Debug.WriteLine ("[Call] - not registered (got NotRegisteredException:{0})", U.InnerExMessage (nrex));
				return ReturnValue.NotRegistered;
			} catch (MomNetworkException neex) {
				Debug.WriteLine ("[Call] - network error (got MomNetworkException:{0})", U.InnerExMessage (neex));
				return ReturnValue.NetworkError;
			} catch (Exception ex) {
				Debug.WriteLine ("[Call] - exception {0}({1})", U.ExType (ex), U.InnerExMessage (ex));
				return ReturnValue.Error;
			}

		}

		public async Task<int> Cancel ()
		{
			Debug.WriteLine ("[Call] - cancelling the call");

			await Task.Run (() => {
				_cancelation.Cancel ();
				Debug.WriteLine ("[Call] - cancelled");
			});

			return ReturnValue.Cancelled;
		}


		private async Task<int> Run (CancellationToken token)
		{
			Debug.WriteLine ("[Call] -  running");
			NetworkArguments netArgs = ValidateValues ();
			INetworkLink _network = App.Container.Resolve<INetworkLink> ();
			var client = await _network.GetNewConnection (netArgs, token);

			var version = await DoHandshake (client, token);
			Debug.WriteLine ("handshace version: " + version);
			//later - fix different versions of protocol
			var flashCommand = BuildRegisterCommand ("1234");//BuildFlashCommand ();
			Debug.WriteLine ("flashcommand: " + flashCommand);
			await client.Send (flashCommand);
			Debug.WriteLine ("after send ");
			var publicKey = await client.Receive (token);
			Debug.WriteLine ("public key: " + publicKey);
			return 0;
		}

		private async Task<string> DoHandshake (IConnectedNetworkClient client, CancellationToken token)
		{
			Debug.WriteLine ("[Call] - doing the handshake");

			await client.Send ("hello", token);
			string answer = await client.Receive (token);
			var answerList = answer.Split (':');

			Debug.WriteLine ("[Call] -  received: " + answerList [0] + ":" + answerList [1]);

			if (!"welcome".Equals (answerList [0].Trim ())) {
				throw new MomException ("Not welcome!");
			}
			return answerList [1];
		}

		private string BuildFlashCommand ()
		{
			Debug.WriteLine ("[Call] -  bulding command");
			int flash_time = _settings.GetCallTime ();
			bool blink = _settings.GetBlinkOrDefault ();
			int intervall_time = blink == false ? 0 : _settings.GetIntervallTimeOrDefault ();
			string random = _cryptoService.GetRandomString ();

			string flash_command = String.Format ("{0}:{1}:{2}:{3}", flash_time, blink, intervall_time, random);
			string crypto_command;
			try {
				crypto_command = _cryptoService.EncodeRSA (_settings.GetServerPublicKey (), flash_command);
			} catch (MomSqlException ex) {
				if (ex.ErrorCode == MomSqlException.NOT_FOUND) {
					// public key not found, throw register exception
					var x = BuildRegisterCommand ("1234");
					throw new MomNotRegisteredException ("App not registered", ex);

				}
				Debug.WriteLine ("Something is wrong with the DB: " + ex.Message);
				throw ex;
			}

			string command = String.Format ("command:{0}", crypto_command);

			return command;
		}

		private string BuildRegisterCommand (string password)
		{
			//1234
			//81dc9bdb52d04dc20036dbd8313ed055
			var md5sum = _cryptoService.GenerateHash (password);
			Debug.WriteLine ("register 1: " + md5sum);
			var command = String.Format ("{0}:{1}", md5sum, _cryptoService.GetRandomString ());
			Debug.WriteLine ("register 2: " + command);
			var crypto_command = _cryptoService.EncodeAES (md5sum, command, Defaults.BLOCKSIZE, Defaults.PADDING);
			Debug.WriteLine ("register 3: " + crypto_command);
			return String.Format ("register:{0}", crypto_command);
		}

		private NetworkArguments ValidateValues ()
		{
			#if DEVEL
			if (_settings.GetIP ().Equals (App.DefaultIp) == true) {
				throw new MomNotRegisteredException (String.Format ("IP is no valid ({0}", App.DefaultIp));
			}
			if (_settings.GetCallTime () < 1) {
				throw new MomNotRegisteredException ("Call time is no valid");
			}
			#endif

			NetworkArguments netArg = new NetworkArguments ();
			netArg.Ip = _settings.GetIP ();
			netArg.Port = _settings.GetPort ();
			netArg.ReceiveTimeout = _settings.GetNetworkTimeoutSeconds ();
			netArg.SendTimeout = _settings.GetNetworkTimeoutSeconds ();
			netArg.NoDelay = true;
			netArg.LingerArguments = new MomLinger {
				Enable = true,
				Timeout = 1
			};
			netArg.ConnectTimeout = _settings.GetConnectTimeOut ();

			return netArg;
		}


	}
}

