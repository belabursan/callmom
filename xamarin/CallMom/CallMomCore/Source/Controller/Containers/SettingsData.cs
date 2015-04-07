﻿using System;
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

		public SettingsData (string ip, string port, string timeout)
		{
			IP = ip;
			try {
				Port = port.AsInteger ();
			} catch (Exception ex) {
				Port = -1;
			}
			try {
				TimeoutSec = timeout.AsInteger ();
			} catch (Exception ex) {
				TimeoutSec = -1;
			}
			int isValid = (int)Validate ();
			if (CORRECT != isValid) {
				throw new MomArgumentException (isValid, "Invalid argument(s)", null);
			}
		}

		public SettingsData (string ip, int port, int timeout)
		{
			IP = ip;
			Port = port;
			TimeoutSec = timeout;
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

