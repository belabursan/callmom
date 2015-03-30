using System;
using System.Text;
using System.Diagnostics;

namespace CallMomCore
{
	public static class Extensions
	{
		public static byte[] AsBytes (this string data)
		{
			try {
				return Encoding.UTF8.GetBytes (data);	
			} catch (Exception ex) {
				Debug.WriteLine ("exception in AsBytes extension");
				throw;
			}
		}

		public static string AsString (this byte[] data)
		{
			try {
				return Encoding.UTF8.GetString (data, 0, data.Length);
			} catch (Exception ex) {
				Debug.WriteLine ("exception in AsString extension");
				throw;
			}
		}
	}
}

