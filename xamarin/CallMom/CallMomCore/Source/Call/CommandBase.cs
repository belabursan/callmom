using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Autofac;

namespace CallMomCore
{
	public abstract class CommandBase : ICommand
	{
		protected readonly ISettingsService _settings;
		protected readonly CancellationTokenSource _cancelation;
		protected readonly ICryptoService _cryptoService;

		protected CommandBase ()
		{
			_settings = App.Container.Resolve<ISettingsService> ();
			_cryptoService = App.Container.Resolve<ICryptoService> ();
			_cancelation = new CancellationTokenSource ();
		}

		public virtual int Cancel ()
		{
			Debug.WriteLine ("[BaseCommand] - cancelling the call");

			_cancelation.Cancel ();
			Debug.WriteLine ("[BaseCommand] - cancelled");

			return ReturnValue.Cancelled;
		}

		public virtual async Task<int> ExecuteAsync ()
		{
			Debug.WriteLine ("[BaseCommand] - executing command");
			try {
				return await Run (_cancelation.Token);
			} catch (OperationCanceledException ocex) {
				Debug.WriteLine ("[BaseCommand] - cancelled (got OperationCanceledException:{0})", U.InnerExMessage (ocex));
				return ReturnValue.Cancelled;
			} catch (MomNetworkException neex) {
				Debug.WriteLine ("[BaseCommand] - network error (got MomNetworkException:{0})", U.InnerExMessage (neex));
				return ReturnValue.NetworkError;
			} catch (Exception ex) {
				Debug.WriteLine ("[BaseCommand] - general exception {0}({1})", U.ExType (ex), U.InnerExMessage (ex));
				return ReturnValue.Error;
			}

		}

		protected abstract Task<int> Run (CancellationToken token);
	}
}

