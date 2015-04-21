using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace CallMomCore
{
	/// <summary>
	/// Extensions.
	/// Holds extensions as converting strings, bytes and enums
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Converts a string encoded as UTF-8 to a byte array
		/// </summary>
		/// <returns>byte array</returns>
		/// <param name="data"> String to convert</param>
		/// <exception cref="Exception">If converting string has failed</exception>
		public static byte[] AsBytes (this string data)
		{
			try {
				return Encoding.UTF8.GetBytes (data);	
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsBytes extension: {0}", ex.Message);
				throw;
			} 
		}

		/// <summary>
		/// Takes a base64 encoded string and converts it to a byte array
		/// </summary>
		/// <returns>The bytes from base64.</returns>
		/// <param name="data">Data.</param>
		public static byte[] AsBytesFromBase64 (this string data)
		{
			try {
				return Convert.FromBase64String (data);
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsBytesFromBase64 extension: {0}", ex.Message);
				throw;
			} 
		}

		/// <summary>
		/// Takes a UTF-8 encoded string and converts it to a byte array
		/// </summary>
		/// <returns>byte array</returns>
		/// <param name="data"> String to convert</param>
		/// <exception cref="Exception">If converting the string has failed</exception>
		public static byte[] AsBase64Bytes (this string data)
		{
			try {
				return Encoding.Unicode.GetBytes (data);
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsBase64Bytes extension: {0}", ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Converts a byte array to string encoded as UTF-8 
		/// </summary>
		/// <returns>The bytes as string.</returns>
		/// <param name="data">Byte array to convert</param>
		/// <exception cref="Exception">If converting the byte array has failed</exception>
		/// <param name = "offset">offset</param>
		/// <param name = "length">length</param>
		public static string AsString (this byte[] data, int offset, int length)
		{
			try {
				return Encoding.UTF8.GetString (data, offset, length);
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsString extension: {0}", ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Converts a byte array to string encoded as UTF-8 
		/// </summary>
		/// <returns>The bytes as string.</returns>
		/// <param name="data">Byte array to convert</param>
		/// <exception cref="Exception">If converting the byte array has failed</exception>

		public static string AsString (this byte[] data)
		{
			return data.AsString (0, data.Length);
		}


		/// <summary>
		/// Converts a byte array to string encoded as Base64
		/// </summary>
		/// <returns>The bytes as string.</returns>
		/// <param name="data">Byte array to convert</param>
		/// <exception cref="Exception">If converting the byte array has failed</exception>
		public static string AsBase64String (this byte[] data)
		{
			return data.AsBase64String (0, data.Length);
		}

		/// <summary>
		/// Converts a byte array to string encoded as UTF-16 (Unicode)
		/// </summary>
		/// <returns>The bytes as string.</returns>
		/// <param name="data">Byte array to convert</param>
		/// <param name = "offset">index of first byte</param>
		/// <param name = "length">length to convert</param>
		/// <exception cref="Exception">If converting the byte array has failed</exception>
		public static string AsBase64String (this byte[] data, int offset, int length)
		{
			try {
				return Convert.ToBase64String (data, offset, length);
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsBase64String extension: {0}", ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Converts a byte array to a string representing the bytes as hexadecimals
		/// </summary>
		/// <returns>The hex string.</returns>
		/// <param name="bytes">Bytes to convert</param>
		/// <param name = "separator">string to setbetween every byte represantition</param>
		/// <exception cref="Exception">If converting the byte array has failed</exception>
		public static string AsHexString (this byte[] bytes, string separator = "")
		{
			try {
				return String.Concat (bytes.Select (b => String.Format ("{0:x2}{1}", b, separator)));
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in ToHexString extension: {0}", ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Converts a string to integer.
		/// </summary>
		/// <returns>Integer value of the string.</returns>
		/// <param name="value">string to convert.</param>
		public static int AsInteger (this string value)
		{
			try {
				return int.Parse (value);
			} catch (Exception ex) {
				Debug.WriteLine ("Exception in AsInteger extension: {0}", ex.Message);
				throw;
			}
		}
	}
}

