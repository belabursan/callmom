﻿using System;

namespace CallMomCore
{
	public class SettingService : ISettingsService
	{
		private readonly ISQLiteLink _sql;

		private const int CALLTIME = 0;
		private const int IP = 1;
		private const int PORT = 2;
		private const int NETWORKTIMEOUT = 3;
		private const int CONNECTTIMEOUT = 4;
		private const int BLINK = 5;
		private const int INTERVALLTIME = 6;
		private const int PUBLICKEY = 7;
		private const int INITIATED = 8;

		public SettingService (ISQLiteLink sql)
		{
			sql.DB ().CreateTable<Settings> ();
			_sql = sql;
		}

		#region ISettingsService implementation

		public void ResetTODefaults ()
		{
			_sql.CleanupDB ();
			InsertCallTime (Defaults.CALLTIMEOUT);
			InsertIP (Defaults.IP);
			InsertPort (Defaults.PORT);
			InsertNetworkTimeoutSeconds (Defaults.NETTIMEOUT);
			InsertConnectTimeOut (Defaults.CONNECTTIMEOUT);
		}

		public void InsertCallTime (int time)
		{
			Insert (CALLTIME, time.ToString ());
		}

		public int GetCallTime ()
		{
			return ValueAsInteger (CALLTIME);
		}

		public void InsertIP (string ip)
		{
			Insert (IP, ip);
		}

		public string GetIP ()
		{
			return ValueAsString (IP);
		}

		public int GetPort ()
		{
			return ValueAsInteger (PORT);
		}

		public void InsertPort (int port)
		{
			Insert (PORT, port.ToString ());
		}

		public void InsertNetworkTimeoutSeconds (int timeout)
		{
			Insert (NETWORKTIMEOUT, timeout.ToString ());
		}

		public int GetNetworkTimeoutSeconds ()
		{
			return ValueAsInteger (NETWORKTIMEOUT);
		}

		public void InsertConnectTimeOut (int timeout)
		{
			Insert (CONNECTTIMEOUT, timeout.ToString ());
		}

		public int GetConnectTimeOut ()
		{
			return ValueAsInteger (CONNECTTIMEOUT);
		}

		public int GetIntervallTime ()
		{
			return ValueAsInteger (INTERVALLTIME);
		}

		public int GetIntervallTimeOrDefault (int defaultValue = 0)
		{
			try {
				return GetIntervallTime ();
			} catch (MomSqlException ex) {
				if (ex.ErrorCode == MomSqlException.NOT_FOUND) {
					return defaultValue;
				}
				throw ex;
			}
		}

		public void InsertIntervallTime (int time)
		{
			Insert (INTERVALLTIME, time.ToString ());
		}

		public bool GetBlink ()
		{
			return ValueAsBoolean (BLINK);
		}

		public bool GetBlinkOrDefault (bool defaultValue = false)
		{
			try {
				return GetBlink ();
			} catch (MomSqlException ex) {
				if (ex.ErrorCode == MomSqlException.NOT_FOUND) {
					return defaultValue;
				}
				throw ex;
			}
		}

		public void InsertBlink (bool doBlink)
		{
			Insert (BLINK, doBlink.ToString ());
		}

		public byte[] GetServerPublicKey ()
		{
			return ValueAsString (PUBLICKEY).AsBytes ();
		}

		public void InsertServerPublicKey (byte[] publicKey)
		{
			Insert (PUBLICKEY, publicKey.AsString (0, publicKey.Length));
		}

		private bool GetInitiated ()
		{
			try {
				return ValueAsBoolean (INITIATED);
			} catch (MomSqlException ex) {
				if (ex.ErrorCode == MomSqlException.NOT_FOUND) {
					return false;
				}
				throw ex;
			}
		}

		void InsertInitiated (bool initiated)
		{
			Insert (BLINK, initiated.ToString ());
		}

		public void FirstTimeInit ()
		{
			if (!GetInitiated ()) {
				InsertCallTime (Defaults.CALLTIMEOUT);
				InsertIP (Defaults.IP);
				InsertPort (Defaults.PORT);
				InsertNetworkTimeoutSeconds (Defaults.NETTIMEOUT);
				InsertConnectTimeOut (Defaults.CONNECTTIMEOUT);
				InsertInitiated (true);
			}
		}

		#endregion

		private int ValueAsInteger (int key)
		{
			return int.Parse (_sql.GetItemById<Settings> (key).Value);
		}

		private bool ValueAsBoolean (int key)
		{
			return Boolean.Parse (_sql.GetItemById<Settings> (key).Value);
		}

		private string ValueAsString (int key)
		{
			return _sql.GetItemById<Settings> (key).Value;
		}

		private void Insert (int key, string value)
		{
			_sql.InsertOrUpdateItem<Settings> (new Settings {
				Key = key,
				Value = value
			});
		}
	}
}

