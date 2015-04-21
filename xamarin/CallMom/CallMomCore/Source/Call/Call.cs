using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Autofac;
using System.Threading;

namespace CallMomCore
{
	public class Call : CallBase
	{

		#region implemented abstract members of CommandBase


		protected override async Task<int> Run (string value = default(string), CancellationToken token = default(CancellationToken))
		{
			Debug.WriteLine ("[Call] -  running");
			IConnectedNetworkClient client = null;
			try {
				var flashCommand = BuildFlashCommand ();
				client = await Connect (token);
				var version = await DoHandshake (client, token);
				Debug.WriteLine ("Protocol version: {0}\ncommand: {1} ", version, flashCommand);
				// todo call sendkey, check answer lrc
				await client.SendAsync (flashCommand);
			} finally {
				if (client != null) {
					client.Close ();
				}
			}
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
				crypto_command = _cryptoService.EncodeRSA (_settings.GetServerPublicKey (), flash_command.AsBytes ());
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

