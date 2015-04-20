﻿using System;
using CallMomCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace CallMomiOS
{
	public class CryptoFactory : ICryptoFactory
	{
		private readonly SHA256 _sha256Hasher;
		private int _blockSize;

		public CryptoFactory (int blockSize = Defaults.BLOCKSIZE)
		{
			_blockSize = blockSize;
			_sha256Hasher = SHA256.Create ();
		}

		#region ICryptoFactory implementation

		public byte[] GetSha256Hash (string data)
		{
			return _sha256Hasher.ComputeHash (data.AsBytes ());
		}

		public string EncodeAES (byte[] key, string data)
		{
			byte[] dataBytes = Pad (data, _blockSize);

			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider ()) {
				aes.BlockSize = _blockSize * 8;
				aes.KeySize = Defaults.KEYSIZE * 8;

				aes.Key = key;
				aes.IV = Defaults.AESIV;

				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.None;

				using (ICryptoTransform encryptor = aes.CreateEncryptor ()) {
					using (MemoryStream memoStream = new MemoryStream ()) {
						using (CryptoStream cryptoStream = new CryptoStream (memoStream, encryptor, CryptoStreamMode.Write)) {
							cryptoStream.Write (dataBytes, 0, dataBytes.Length);
							cryptoStream.FlushFinalBlock ();
							return memoStream.ToArray ().AsBase64String ();
						}
					}
				}
			}
		}


		public string EncodeRSA (byte[] key, byte[] data)
		{
			try {
				using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider ()) {
					RSAParameters RSAKey = new RSAParameters ();
					RSAKey.Modulus = key;
					RSAKey.Exponent = new byte[]{ 1, 0, 1 };

					RSA.ImportParameters (RSAKey); 
					Debug.WriteLine ("___DATA__len:{0}_data: {1}", data.Length, data.AsHexString ());
					Debug.WriteLine ("___KEY__len:{0}_data: {1}", key.Length, key.AsHexString ());
					var encryptedData = RSA.Encrypt (data, true);

					var x = encryptedData.ToArray ();
					Debug.WriteLine ("_____{0}_: {1}", x.Length, x.AsHexString ());
					var z = x.AsBase64String ();
					return z;
					
				}
			} catch (CryptographicException e) {
				Console.WriteLine ("[Crypto] - exception in encryptRSA: " + e.Message);
				throw e;
			}
		}

		#endregion

		private byte[] Pad (string data, int blockSize)
		{
			byte[] dataBytes = data.AsBytes ();
			int dataLength = dataBytes.Length;
			int padLength = blockSize;

			if (dataLength < blockSize) {
				padLength -= dataLength;
			} else {
				int modulus = dataLength % blockSize;
				if (modulus != 0) {
					padLength -= modulus;
				}
			}
			int newLength = dataLength + padLength;
			Array.Resize<byte> (ref dataBytes, newLength);
			byte pad = (byte)(0x0F & (byte)padLength);

			for (; dataLength < newLength;) {
				dataBytes [dataLength++] = pad;
			}
			return dataBytes;
		}

	}
}

