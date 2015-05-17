using System;
using CallMomCore;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text;

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
					Debug.WriteLine ("\n___DATAasHex__len:{0}_data: {1}", data.Length, data.AsHexString ());
					Debug.WriteLine ("___KEYasHex__len:{0}:{1}_data: {2}", key.Length, key.Length * 8, key.AsHexString ());

					if (token != default(CancellationToken))
						token.ThrowIfCancellationRequested ();
					
					var z = RSA.Encrypt (data, true).ToArray ();

					//Array.Reverse (x);
					Debug.WriteLine ("\n\n___ENCasHex__len:{0}_data: {1}", z.Length, z.AsHexString ());
					var encryptedData = z.AsBase64String ();
					Debug.WriteLine ("\n_____64byteStr-{0}_: {1}", encryptedData.Length, encryptedData);

					return encryptedData;
					
				}
			} catch (CryptographicException e) {
				Console.WriteLine ("[Crypto] - exception in encryptRSA: " + e.Message);
				throw e;
			}
		}


		#endregion

		private  RSAParameters ParseKeyFromString (string raw_key)
		{
			string RSA_XML_KEY = "<?xml version=\"1.0\" encoding=\"utf-16\"?><RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Exponent>AQAB</Exponent><Modulus>{0}</Modulus></RSAParameters>";

			string temp = raw_key;// Encoding.ASCII.GetString (System.Convert.FromBase64String (raw_key));
			string t = temp;
			int s = temp.IndexOf ('\n') + 1;
			int l = temp.LastIndexOf ('\n');
			temp = temp.Substring (s, temp.Length - s - (temp.Length - l));
			try {

				var sr = new System.IO.StringReader (string.Format (RSA_XML_KEY, temp.Replace ("\n", "")));
				var xs = new System.Xml.Serialization.XmlSerializer (typeof(RSAParameters));
				var publickey = (RSAParameters)xs.Deserialize (sr);
				return publickey;
			} catch (Exception) {
				return new RSAParameters ();
			}
		}

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

