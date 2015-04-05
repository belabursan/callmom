using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Autofac;
using System.Threading;

namespace CallMomCore
{
	public class Call : CommandBase
	{
		public override async Task<int> ExecuteAsync ()
		{
			try {
				return await Run (_cancelation.Token);	
			} catch (OperationCanceledException ocex) {
				Debug.WriteLine ("[Call] - cancelled (got OperationCanceledException:{0})", U.InnerExMessage (ocex));
				return ReturnValue.Cancelled;
			} catch (MomNetworkException neex) {
				Debug.WriteLine ("[Call] - network error (got MomNetworkException:{0})", U.InnerExMessage (neex));
				return ReturnValue.NetworkError;
			} catch (MomNotRegisteredException nrex) {
				Debug.WriteLine ("[Call] - not registered (got NotRegisteredException:{0})", U.InnerExMessage (nrex));
				return ReturnValue.NotRegistered;
			} catch (Exception ex) {
				Debug.WriteLine ("[Call] - exception {0}({1})", U.ExType (ex), U.InnerExMessage (ex));
				return ReturnValue.Error;
			}

		}

		#region implemented abstract members of CommandBase


		protected override async Task<int> Run (CancellationToken token)
		{
			Debug.WriteLine ("[Call] -  running");
			NetworkArguments netArgs = ValidateValues ();
			INetworkLink _network = App.Container.Resolve<INetworkLink> ();
			var client = await _network.GetNewConnection (netArgs, token);

			var version = await DoHandshake (client, token);
			Debug.WriteLine ("Protocol version: " + version);
			//later - fix different versions of protocol

			// todo remove 
			var flashCommand = BuildRegisterCommand ("1234");//BuildFlashCommand ();
			Debug.WriteLine ("flashcommand: " + flashCommand);
			await client.Send (flashCommand);
			var publicKey = await client.Receive (token);
			Debug.WriteLine ("public key: " + publicKey);
			return 0;
		}

		#endregion

		private async Task<string> DoHandshake (IConnectedNetworkClient client, CancellationToken token)
		{
			Debug.WriteLine ("[Call] - starting handshake");

			await client.Send ("hello", token);
			string answer = await client.Receive (token);
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
			Debug.WriteLine ("[Call] - handshake: {0}", answerHeader);
			return answerList [1];
		}

		private string BuildFlashCommand ()
		{
			Debug.WriteLine ("[Call] -  bulding command");
			int flash_time = _settings.GetCallTime ();
			bool blink = _settings.GetBlinkOrDefault ();
			int intervall_time = blink == false ? 0 : _settings.GetIntervallTimeOrDefault ();
			string random = _cryptoService.GetRandomString (20, 40);

			string flash_command = String.Format ("{0}:{1}:{2}:{3}", flash_time, blink, intervall_time, random);
			string crypto_command;
			try {
				crypto_command = _cryptoService.EncodeRSA (_settings.GetServerPublicKey (), flash_command);
			} catch (MomSqlException ex) {
				if (ex.ErrorCode == MomSqlException.NOT_FOUND) {
					// public key not found, throw register exception
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
			Debug.WriteLine ("[Call] - building register command");
			var sha256Hash = _cryptoService.GetSha256Hash (password);
			string random = _cryptoService.GetRandomString (32, 64);
			//Debug.WriteLine ("random: " + random);

			var command = String.Format ("{0}:{1}", sha256Hash.AsHexString (), random);
			var crypto_command = _cryptoService.EncodeAES (sha256Hash, command);

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

