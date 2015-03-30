using System;
using CallMomCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;

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
			return ToHex (_md5Hasher.ComputeHash (Encoding.UTF8.GetBytes (data)));
		}

		public string EncodeAES (string keyStr, string data, int blockSize, string padding)
		{
			byte[] dataBytes = data.AsBytes ();
			byte[] encryptedBytes = null;

			using (MemoryStream memoryStream = new MemoryStream ()) {
				using (RijndaelManaged AES = new RijndaelManaged ()) {
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes (keyStr.AsBytes (), Defaults.SALT.AsBytes (), 1000);
					//AES.Key = key.GetBytes (AES.KeySize / 8);
					AES.Key = key.GetBytes (32);
					AES.IV = key.GetBytes (Defaults.BLOCKSIZE);
					AES.Mode = CipherMode.CBC;

					using (var cs = new CryptoStream (memoryStream, AES.CreateEncryptor (), CryptoStreamMode.Write)) {
						cs.Write (dataBytes, 0, dataBytes.Length);
						cs.Close ();
					}
					encryptedBytes = memoryStream.ToArray ();
				}
			}
			return encryptedBytes.AsString ();
		}

		#endregion

		public static string ToHex (byte[] bytes)
		{
			return String.Concat (bytes.Select (b => string.Format ("{0:x2}", b)));
		}
	}
}

