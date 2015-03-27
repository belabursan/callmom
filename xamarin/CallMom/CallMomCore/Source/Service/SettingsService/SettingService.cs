using System;

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

		public SettingService (ISQLiteLink sql)
		{
			sql.DB ().CreateTable<Settings> ();
			_sql = sql;
		}

		#region ISettingsService implementation

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

		#endregion

		private int ValueAsInteger (int key)
		{
			return int.Parse (_sql.GetItemById<Settings> (key).Value);
		}

		private string ValueAsString (int key)
		{
			return _sql.GetItemById<Settings> (key).Value;
		}

		private void Insert (int key, string value)
		{
			Settings setting = new Settings {
				Key = key,
				Value = value
			};
			_sql.InsertOrUpdateItem<Settings> (setting);
		}
	}
}

