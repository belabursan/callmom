using System;
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

