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
		private readonly IStateService _stateMachine;
		private readonly CancellationTokenSource _cancelation;


		public Call ()
		{
			_settings = App.Container.Resolve<ISettingsService> ();
			_stateMachine = App.Container.Resolve<IStateService> ();
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
			} finally {
				_stateMachine.Reset ();
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
			client.Send ("kkk", token);

			//Debug.WriteLine ("[Call] -  running: dissconnected: " + b.ToString ());
			await Task.Delay (8000, token);

			return 7;
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

