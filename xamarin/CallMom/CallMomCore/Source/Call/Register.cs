using System;
using System.Threading;
using Autofac;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CallMomCore
{
	public class Register : CallBase
	{

		#region implemented abstract members of CommandBase

		protected override async Task<int> Run (string value = default(string), CancellationToken token = default(CancellationToken))
		{
			Debug.WriteLine ("[Register] -  running");

			var registerCommand = BuildRegisterCommand (value);
			var client = await Connect (token);
			var version = await DoHandshake (client, token);
			Debug.WriteLine ("Protocol version: {0}\ncommand: {1}", version, registerCommand);

			await client.SendAsync (registerCommand);
			var publicKey = await client.ReceiveAsBytesAsync (token);
			if (publicKey == null || publicKey.Length < 32) {
				throw new MomNotRegisteredException ("public key is corrupt or null");
			}
			_settings.InsertServerPublicKey (publicKey);

			//Debug.WriteLine ("public key: " + _settings.GetServerPublicKey ().AsString (0, publicKey.Length));
			return 0;
		}

		#endregion

		private string BuildRegisterCommand (string password)
		{
			Debug.WriteLine ("[Register] - building command");
			var sha256Hash = _cryptoService.GetSha256Hash (password);
			string random = _cryptoService.GetRandomString (32, 64);
			//Debug.WriteLine ("random: " + random);

			var command = String.Format ("{0}:{1}", sha256Hash.AsHexString (), random);
			var crypto_command = _cryptoService.EncodeAES (sha256Hash, command);

			return String.Format ("register:{0}", crypto_command);
		}
	}
}

