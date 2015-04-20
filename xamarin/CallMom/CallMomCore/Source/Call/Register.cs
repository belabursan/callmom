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
			byte[] sha256Hash = _cryptoService.GetSha256Hash (value);

			var registerCommand = BuildRegisterCommand (sha256Hash);
			var client = await Connect (token);
			var version = await DoHandshake (client, token);
			Debug.WriteLine ("Protocol version: {0}\ncommand: {1}", version, registerCommand);

			await client.SendAsync (registerCommand);
			string answer = await client.ReceiveAsync (token);
			byte[] publicKey = HandleRegisterAnswer (answer, sha256Hash);
			_settings.InsertServerPublicKey (publicKey);

			//Debug.WriteLine ("public key: " + _settings.GetServerPublicKey ().AsString (0, publicKey.Length));
			return 0;
		}

		#endregion

		private string BuildRegisterCommand (byte[] sha256Hash)
		{
			Debug.WriteLine ("[Register] - building command");
			string random = _cryptoService.GetRandomString (32, 64);
			//Debug.WriteLine ("random: " + random);

			var command = String.Format ("{0}:{1}", sha256Hash.AsHexString (), random);
			var crypto_command = _cryptoService.EncodeAES (sha256Hash, command);

			return String.Format ("{0}{1}{2}", Protocol.Register, Protocol.SPLITTER, crypto_command);
		}

		byte[] HandleRegisterAnswer (string answer, byte[] key)
		{
			string[] parts = answer.Split (Protocol.SPLITTER);
			if (Protocol.XCHANGEKEY.Equals (parts [0])) {
				string decrypted = _cryptoService.DecodeAES (key, parts [1]);
				string[] body = decrypted.Split (Protocol.SPLITTER);
				return body [0].AsBytes ();
			}
			throw new MomProtocolException ("Got wrong answer when registering: " + answer);
		}
	}
}

