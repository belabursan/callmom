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
			byte[] encryptedBytes = null;

			using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider ()) {
				aesAlg.Key = keyStr.AsBytes ();
				aesAlg.IV = Defaults.SALT.AsBytes ();
				aesAlg.BlockSize = Defaults.BLOCKSIZE * 8;
				aesAlg.KeySize = Defaults.KEYSIZE * 8;
				aesAlg.Mode = CipherMode.CBC;
				aesAlg.Padding = PaddingMode.Zeros;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor (aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption. 
				using (MemoryStream msEncrypt = new MemoryStream ()) {
					using (CryptoStream csEncrypt = new CryptoStream (msEncrypt, encryptor, CryptoStreamMode.Write)) {
						using (StreamWriter swEncrypt = new StreamWriter (csEncrypt)) {

							//Write all data to the stream.
							swEncrypt.Write (data);
						}
						encryptedBytes = msEncrypt.ToArray ();
					}
				}

			}
			return encryptedBytes.AsBase64String ();
			/*
			using (MemoryStream memoryStream = new MemoryStream ()) {
				using (RijndaelManaged AES = new RijndaelManaged ()) {
					Rfc2898DeriveBytes key = new Rfc2898DeriveBytes (keyStr, Defaults.SALT.AsBytes ());

					AES.KeySize = Defaults.KEYSIZE * 8;
					AES.BlockSize = Defaults.BLOCKSIZE * 8;
					AES.Key = key.GetBytes (Defaults.KEYSIZE);
					AES.IV = Defaults.SALT.AsBytes ();
					AES.Mode = CipherMode.CBC;
					AES.Padding = PaddingMode.Zeros;
					//var key = new Rfc2898DeriveBytes (keyStr.AsBytes (), Defaults.SALT.AsBytes (), 0);
					//AES.Key = key.GetBytes (AES.KeySize / 8);

					using (var cs = new CryptoStream (memoryStream, AES.CreateEncryptor (), CryptoStreamMode.Write)) {
						cs.Write (dataBytes, 0, dataBytes.Length);
						cs.Close ();
					}
					encryptedBytes = memoryStream.ToArray ();
				}
			}
			return encryptedBytes.AsBase64String ();
			*/
		}

		#endregion

		public static string ToHex (byte[] bytes)
		{
			return String.Concat (bytes.Select (b => string.Format ("{0:x2}", b)));
		}
	}
}

