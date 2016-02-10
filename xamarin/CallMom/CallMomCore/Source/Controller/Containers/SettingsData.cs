using System;
using System.Text.RegularExpressions;

namespace CallMomCore
{
	public class SettingsData
	{
		public const byte CORRECT = 0x00;
		public const byte WRONG_IP = 0x04;
		public const byte WRONG_PORT = 0x02;
		public const byte WRONG_TIMEOUT = 0x01;

		public SettingsData ()
		{
		}

		public SettingsData (string ip, string port, float timeoutSec)
		{
			IP = ip;
			try {
				Port = port.AsInteger ();
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine ("could not parse port (" + port + "): " + ex.Message);
				Port = -1;
			}
			TimeoutSec = (int)timeoutSec;
			int isValid = (int)Validate ();
			if (CORRECT != isValid) {
				throw new MomArgumentException (isValid, "Invalid argument(s)", null);
			}
		}

		/// <summary>
		/// Validates the values to fit the requirements fro ip port and timeout
		/// </summary>
		/// <returns>1000 if successed, error number if failed
		public byte Validate ()
		{
			byte errorCode = CORRECT;

			if (!ValidateIp (IP)) {
				errorCode |= WRONG_IP;
			}
			if (Port < 2001 || Port > 65000) {
				errorCode |= WRONG_PORT;
			}
			if (TimeoutSec < 0 || TimeoutSec > 60) {
				errorCode |= WRONG_TIMEOUT;
			}

			return errorCode;
		}

		public string IP { get; set; }

		public int Port { get; set; }

		public int TimeoutSec { get; set; }

		public bool IsRegistred { get; set; }

		private static bool ValidateIp (string ip)
		{
			bool isValid = false;
			if (!String.IsNullOrEmpty (ip)) {
				Regex check = new Regex (@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$");
				isValid = check.IsMatch (ip);
			}
			return isValid;

		}
	}
}

