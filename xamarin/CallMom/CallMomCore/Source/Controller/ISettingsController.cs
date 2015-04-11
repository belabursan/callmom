﻿using System;
using System.Threading.Tasks;

namespace CallMomCore
{
	public interface ISettingsController
	{
		Task<int> DoRegister ();

		void DoReset ();

		SettingsData GetSettings ();

		void SetSettings (SettingsData arguments);

		bool IsRegistered ();

		string GetAbout ();
	}
}
