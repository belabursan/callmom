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


		public Call ()
		{
			_settings = App.Container.Resolve<ISettingsService> ();
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
			client.Send ("hello", token);
			string answer = await client.Receive (token);
			var answerList = answer.Split (':');

			Debug.WriteLine ("[Call] -  received: " + answerList [0]);
			if ("welcome".Equals (answerList [0].Trim ())) {
				var protocol = answerList [1];
				//todo - handle different protocols later...
				var x = BuildFlashCommand ();
				client.Send (x);
			}


			await Task.Delay (8000, token);

			return 7;
		}

		private string BuildFlashCommand ()
		{
			ICryptoService crypto = App.Container.Resolve<ICryptoService> ();
			Debug.WriteLine ("[Call] -  bulding command");
			int flash_time = _settings.GetCallTime ();
			int intervall_time = _settings.GetIntervallTime ();
			bool blink = _settings.GetBlink ();
			if (blink == false) {
				intervall_time = 0;
			}
			string random = crypto.GetRandomString ();

			string flash_command = String.Format ("{0}:{1}:{2}:{3}", flash_time, blink, intervall_time, random);
			string crypto_command = crypto.EncodeRSA (_settings.GetServerPublicKey (), flash_command);
			string command = String.Format ("command:{0}", crypto_command);

			return command;
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

