using System;
using CallMomCore;
using System.Security.Cryptography;

namespace CallMomiOS
{
	public class CryptoFactory : ICryptoFactory
	{
		private readonly MD5 _md5Hasher;

		public CryptoFactory ()
		{
			_md5Hasher = MD5.Create ();
		}

		#region ICryptoFactory implementation

		public string GetMD5Hash (string data)
		{
			byte[] buffer = Convert.FromBase64String (data);
			var b = _md5Hasher.ComputeHash (buffer);
			return Convert.ToBase64String (b);
		}

		#endregion
	}
}

