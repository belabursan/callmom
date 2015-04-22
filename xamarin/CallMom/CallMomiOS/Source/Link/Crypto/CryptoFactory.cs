using System;
using CallMomCore;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace CallMomiOS
{
	public class CryptoFactory : ICryptoFactory
	{
		private enum CryptoType
		{
			Encrypt,
			Decrypt
		}

		private readonly SHA256 _sha256Hasher;
		private readonly int _blockSize;

		public CryptoFactory (int blockSize = Defaults.BLOCKSIZE)
		{
			_blockSize = blockSize;
			_sha256Hasher = SHA256.Create ();
		}

		#region ICryptoFactory implementation

		public byte[] GetSha256Hash (string data, CancellationToken token = default(CancellationToken))
		{
			if (token != default(CancellationToken))
				token.ThrowIfCancellationRequested ();
			
			return _sha256Hasher.ComputeHash (data.AsBytes ());
		}

		public string EncodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			try {
				return Crypt (key, data, CryptoType.Encrypt, token);
			} catch (CryptographicException cx) {
				Console.WriteLine ("[Crypto] - exception in EncodeAES(): " + cx.Message);
				throw new MomProtocolException (cx.Message, cx);
			}
		}

		public string DecodeAES (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			try {
				return Crypt (key, data, CryptoType.Decrypt, token);
			} catch (CryptographicException cx) {
				Console.WriteLine ("[Crypto] - exception in DecodeAES(): " + cx.Message);
				throw new MomProtocolException (cx.Message, cx);
			}
		}

		public string EncodeRSA (byte[] key, byte[] data, CancellationToken token = default(CancellationToken))
		{
			try {
				using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider ()) {
					RSAParameters RSAKey = new RSAParameters ();
					RSAKey.Modulus = key;
					RSAKey.Exponent = new byte[]{ 1, 0, 1 };

					RSA.ImportParameters (RSAKey); 
					Debug.WriteLine ("___DATA__len:{0}_data: {1}", data.Length, data.AsHexString ());
					Debug.WriteLine ("___KEY__len:{0}_data: {1}", key.Length, key.AsHexString ());

					if (token != default(CancellationToken))
						token.ThrowIfCancellationRequested ();
					
					var encryptedData = RSA.Encrypt (data, true).ToArray ().AsBase64String ();

					Debug.WriteLine ("_____64bytes-{0}_: {1}", encryptedData.Length, encryptedData);

					return encryptedData;
					
				}
			} catch (CryptographicException e) {
				Console.WriteLine ("[Crypto] - exception in encryptRSA: " + e.Message);
				throw e;
			}
		}


		#endregion

		private string Crypt (byte[] key, byte[] data, CryptoType cryptoType, CancellationToken token)
		{
			using (var aes = new AesManaged ()) {
				aes.BlockSize = _blockSize * 8;
				aes.KeySize = Defaults.KEYSIZE * 8;
				ICryptoTransform cryptor = null;

				if (cryptoType == CryptoType.Encrypt) {
					cryptor = aes.CreateEncryptor (key, Defaults.AESIV);
				} else {
					cryptor = aes.CreateDecryptor (key, Defaults.AESIV);
				}
				using (cryptor) {
					using (var memoStream = new MemoryStream ()) {
						using (var cryptoStream = new CryptoStream (memoStream, cryptor, CryptoStreamMode.Write)) {

							if (token != default(CancellationToken))
								token.ThrowIfCancellationRequested ();

							cryptoStream.Write (data, 0, data.Length);
							cryptoStream.FlushFinalBlock ();

							if (cryptoType == CryptoType.Encrypt)
								return memoStream.ToArray ().AsBase64String ();
							else
								return memoStream.ToArray ().AsString ();
						}
					}
				}
			}
		}

		/*
		private static byte[] Pad (byte[] inData, int blockSize, CancellationToken token = default(CancellationToken))
		{
			//todo - check if this is needed...
			byte[] data = (byte[])inData.Clone ();

			int dataLength = data.Length;
			int padLength = blockSize;

			if (dataLength < blockSize) {
				padLength -= dataLength;
			} else {
				int modulus = dataLength % blockSize;
				if (modulus != 0) {
					padLength -= modulus;
				}
			}
			token.ThrowIfCancellationRequested ();

			int newLength = dataLength + padLength;
			Array.Resize<byte> (ref data, newLength);
			byte pad = (byte)(0x0F & (byte)padLength);

			for (; dataLength < newLength;) {
				data [dataLength++] = pad;
			}
			return data;
		}
		*/
	}
}

