using System;

namespace CallMomCore
{
	public interface ISettingsService
	{
		void InsertCallTime (int time);

		int GetCallTime ();

		void InsertIP (string ip);

		string GetIP ();

		int GetPort ();

		void InsertPort (int port);

		void InsertNetworkTimeoutSeconds (int timeout);

		int GetNetworkTimeoutSeconds ();

		int GetConnectTimeOut ();

		void InsertConnectTimeOut (int time);
	}
}

