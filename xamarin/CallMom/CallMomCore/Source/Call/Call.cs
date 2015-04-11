using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Autofac;
using System.Threading;

namespace CallMomCore
{
	public class Call : CallBase
	{
		public override async Task<int> ExecuteAsync (string value = default(string))
		{
			try {
				return await Run (value, _cancelation.Token);
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


		protected override async Task<int> Run (string value = default(string), CancellationToken token = default(CancellationToken))
		{
			Debug.WriteLine ("[Call] -  running");

			var flashCommand = BuildFlashCommand ();
			var client = await Connect (token);
			var version = await DoHandshake (client, token);
			Debug.WriteLine ("Protocol version: {0}\ncommand: {1} ", version, flashCommand);

			await client.SendAsync (flashCommand);

			return 0;
		}

		#endregion

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

	}
}

